using Blacklite.Framework.Features.Describers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.Composition
{
    public interface IFeatureComposition
    {
        int Priority { get; }
        bool IsApplicableTo(IFeatureDescriber describer);
        T Configure<T>(T feature, IFeatureDescriber describer);
    }

    public interface IFeatureComposition<in TFeature>
        where TFeature : class, new()
    {
        int Priority { get; }
        IFeature Configure(TFeature aspect, IFeatureDescriber describer);
    }
}
