using Microsoft.Framework.DependencyInjection;
using Blacklite.Framework.Features.OptionModel;
using Blacklite.Framework.Features.Traits;
using System;
using System.Linq;
using System.Reflection;
using Blacklite.Framework.Features.Aspects;

namespace Blacklite.Framework.Features.Setups
{
    public class OptionsAspectSetup : IAspectSetup
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IFeatureDescriberProvider _describerProvider;
        public OptionsAspectSetup(IServiceProvider serviceProvider, IFeatureDescriberProvider describerProvider)
        {
            _serviceProvider = serviceProvider;
            _describerProvider = describerProvider;
        }

        public int Priority { get; } = int.MaxValue;

        public T Configure<T>(T aspect)
        {
            var traitOptions = aspect as ITraitOptions;
            if (traitOptions != null)
            {
                var describer = _describerProvider.Describers[typeof(T)];
                Type optionsType;
                if (typeof(IAspect).GetTypeInfo().IsAssignableFrom(describer.OptionsTypeInfo))
                {
                    optionsType = typeof(Feature<>).MakeGenericType(describer.OptionsType);
                }
                else
                {
                    optionsType = typeof(IAspectOptions<>).MakeGenericType(describer.OptionsType);
                }
                traitOptions.SetOptions(_serviceProvider.GetService(optionsType));
            }

            return aspect;
        }

        public bool IsApplicableTo(IFeatureDescriber describer)
        {
            return describer.HasOptions;
        }
    }
}
