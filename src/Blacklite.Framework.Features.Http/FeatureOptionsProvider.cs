using Blacklite.Framework.Features.OptionsModel;
using System;

namespace Blacklite.Framework.Features.Http
{
    public class FeatureOptionsProvider : IFeatureOptionsProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public FeatureOptionsProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetOptions(Type optionsType)
        {
            var optionManagerType = typeof(FeatureOptionsManager<>).MakeGenericType(optionsType);
            return ((IFeatureOptions<object>)_serviceProvider.GetService(optionManagerType)).Options;
        }
    }
}
