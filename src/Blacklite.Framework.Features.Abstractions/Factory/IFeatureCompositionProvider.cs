using Blacklite.Framework.Features.Composition;
using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features.Factory
{
    public interface IFeatureCompositionProvider
    {
        IEnumerable<IFeatureComposition> GetComposers<TFeature>()
            where TFeature : class, new();
    }
}
