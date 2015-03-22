using Blacklite.Framework.Features.Describers;
using Microsoft.Framework.ConfigurationModel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Composition
{
    public class ConfigurationFeatureComposer : IFeatureComposition
    {
        private readonly IConfiguration _configuration;
        private readonly Func<IFeatureDescriber, bool> _predicate;

        public ConfigurationFeatureComposer(IConfiguration configuration)
            : this(configuration, describer => describer.TypeInfo.GetCustomAttributes<ConfigurationFeatureAttribute>().Any())
        {
        }

        public ConfigurationFeatureComposer(IConfiguration configuration, Func<IFeatureDescriber, bool> predicate)
        {
            _configuration = configuration;
            _predicate = predicate;
        }

        public int Priority { get; } = 100;

        public T Configure<T>(T feature, IFeatureDescriber describer)
        {
            string value;

            if (describer.HasEnabled)
            {
                if (_configuration.TryGet($"{describer.Type.Name}:IsEnabled", out value))
                {
                    describer.SetIsEnabled(feature, value == true.ToString());
                }
            }

            if (describer.Properties.Any())
            {
                foreach (var property in describer.Properties)
                {

                    if (_configuration.TryGet($"{describer.Type.Name}:Options:{property.Name}", out value))
                    {
                        if (!typeof(IConvertible).GetTypeInfo().IsAssignableFrom(property.Type.GetTypeInfo()))
                            throw new NotImplementedException("Unable to convert from type that doesnt implement ICovnertible");

                        property.SetProperty(feature, Convert.ChangeType(value, property.Type));
                    }
                }
            }

            if (describer.HasOptions && !describer.Options.IsFeature)
            {
                var options = describer.GetOptions<object>(feature);
                foreach (var property in describer.Options.TypeInfo.GetDeclaredProperties())
                {
                    if (_configuration.TryGet($"{describer.Type.Name}:Options:{property.Name}", out value))
                    {
                        if (!typeof(IConvertible).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo()))
                            throw new NotImplementedException("Unable to convert from type that doesnt implement ICovnertible");

                        property.SetValue(options, Convert.ChangeType(value, property.PropertyType));
                    }
                }
            }

            return feature;
        }

        public bool IsApplicableTo(IFeatureDescriber describer) => _predicate(describer);
    }
}
