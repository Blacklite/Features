using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features.Resolvers
{
    public interface IFeatureResolverProvider
    {
        IReadOnlyDictionary<Type, IEnumerable<IFeatureResolverDescriptor>> Resolvers { get; }
    }
}
