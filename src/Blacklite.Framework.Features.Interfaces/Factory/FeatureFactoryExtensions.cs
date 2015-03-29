using Blacklite.Framework.Features.Observables;
using System;
using System.Reflection;

namespace Blacklite.Framework.Features.Factory
{
    public static class FeatureFactoryExtensions
    {
        public static TFeature GetFeature<TFeature>(this IFeatureFactory factory)
            where TFeature : class, new()
        {
            return (TFeature)factory.GetFeature(typeof(TFeature));
        }

        private static MethodInfo _factoryMethod = typeof(IObservableFeatureFactory).GetTypeInfo().GetDeclaredMethod(nameof(IObservableFeatureFactory.GetObservableFeature));
        public static object GetObservableFeature(this IObservableFeatureFactory factory, Type featureType)
        {
            return _factoryMethod.MakeGenericMethod(featureType).Invoke(factory, null);
        }
    }
}
