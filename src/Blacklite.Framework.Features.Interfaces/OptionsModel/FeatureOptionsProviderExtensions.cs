using System;

namespace Blacklite.Framework.Features.OptionsModel
{
    public static class FeatureOptionsProviderExtensions
    {
        public static TOptions GetOptions<TOptions>(this IFeatureOptionsProvider provider)
            where TOptions : class, new()
        {
            return (TOptions)provider.GetOptions(typeof(TOptions));
        }
    }
}
