using Microsoft.Framework.DependencyInjection;
using Blacklite.Framework.Features.OptionsModel;
using System;
using System.Linq;
using System.Reflection;
using Blacklite.Framework.Features.Describers;

namespace Blacklite.Framework.Features.Composition
{
    public class OptionsFeatureComposer : IFeatureComposition, IOptionsFeatureComposer
    {
        private readonly IServiceProvider _serviceProvider;
        public OptionsFeatureComposer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public int Priority { get; } = int.MaxValue;

        public T Configure<T>(T feature, IFeatureDescriber describer)
        {
            var traitOptions = feature as IFeatureOptions;
            if (traitOptions != null)
            {
                object options;
                if (typeof(IFeature).GetTypeInfo().IsAssignableFrom(describer.Options.TypeInfo))
                {
                    var optionsType = typeof(Feature<>).MakeGenericType(describer.Options.Type);
                    options = ((Feature<object>)_serviceProvider.GetService(optionsType)).Value;
                }
                else
                {
                    var optionsType = typeof(IFeatureOptions<>).MakeGenericType(describer.Options.Type);
                    options = ((IFeatureOptions<object>)_serviceProvider.GetService(optionsType)).Options;
                }
                traitOptions.SetOptions(options);
            }

            return feature;
        }

        public bool IsApplicableTo(IFeatureDescriber describer)
        {
            return describer.HasOptions;
        }
    }
}
