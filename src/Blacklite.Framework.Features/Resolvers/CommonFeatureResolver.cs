using System;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Resolvers
{
    public class CommonFeatureResolver : IFeatureResolver
    {
        private static Type[] CommonTypes = typeof(Feature).GetTypeInfo().DeclaredNestedTypes
            .Where(x => x.IsClass)
            .Where(x => x.ImplementedInterfaces.Contains(typeof(IFeature)))
            .Select(x => x.AsType())
            .ToArray();

        public Type GetFeatureType() => null;

        public int Priority { get; } = int.MaxValue;

        public bool CanResolve<T>(IFeatureResolutionContext context) where T : IFeature => CommonTypes.Contains(typeof(T).GetTypeInfo().BaseType);

        public T Resolve<T>() where T : IFeature
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
    }
}
