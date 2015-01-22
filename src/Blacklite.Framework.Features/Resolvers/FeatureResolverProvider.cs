using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Blacklite.Framework.Features.Resolvers
{
    class FeatureResolverProvider : IFeatureResolverProvider
    {
        private static IReadOnlyDictionary<Type, IEnumerable<IFeatureResolverDescriptor>> GetMetadatumResolverDictionary<TResolver>(IEnumerable<TResolver> resolvers)
            where TResolver : IFeatureResolver
        {
            var descriptors = resolvers.Select(x => new FeatureResolverDescriptor(x));
            var globalDescriptors = descriptors.Where(x => x.IsGlobal);

            var dictionary = descriptors
                .Where(x => !x.IsGlobal)
                        .GroupBy(x => x.FeatureType)
                        .ToDictionary(x => x.Key, x =>
                            x.Union(globalDescriptors)
                             .OrderByDescending(z => z.Priority)
                             .Cast<IFeatureResolverDescriptor>()
                             .AsEnumerable());

            return new ReadOnlyDictionary<Type, IEnumerable<IFeatureResolverDescriptor>>(dictionary);
        }

        public FeatureResolverProvider(IEnumerable<IFeatureResolver> resolvers)
        {
            // Null metadatum type isn't invalid, it allows the resolver to
            // operate against all metadatums.
            // For example, a resolver that looks at a persistance store, could possibly
            // resolve every available type of metadatum if it has a value for it.
            Resolvers = GetMetadatumResolverDictionary(resolvers);
        }

        public IReadOnlyDictionary<Type, IEnumerable<IFeatureResolverDescriptor>> Resolvers { get; }
    }

}
