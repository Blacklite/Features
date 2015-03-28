using Blacklite.Framework.Features.Composition;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Blacklite.Framework.Features.Factory
{
    public static class FeatureCompositionProviderExtensions
    {
        public static MethodInfo GetComposersGenericMethod = typeof(IFeatureCompositionProvider).GetTypeInfo().GetDeclaredMethod(nameof(IFeatureCompositionProvider.GetComposers));
        public static IEnumerable<IFeatureComposition> GetComposers(this IFeatureCompositionProvider provider, Type featureType)
        {
            return (IEnumerable<IFeatureComposition>)GetComposersGenericMethod
                .MakeGenericMethod(featureType).Invoke(provider, null);
        }
    }
}
