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

        public bool CanResolve(IFeatureResolutionContext context) => CommonTypes.Contains(context.FeatureType);

        public IFeature Resolve(IFeatureResolutionContext context)
        {
            return (IFeature)Activator.CreateInstance(context.FeatureType);
        }
    }
}
