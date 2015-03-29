using Blacklite.Framework.Features.Describers;
using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features.Composition
{
    public interface IFeatureComposition
    {
        int Priority { get; }
        bool IsApplicableTo(IFeatureDescriber describer);
        T Configure<T>(T feature, IFeatureDescriber describer);
    }

    public interface IPreFeatureComposition : IFeatureComposition { }

    public interface IPostFeatureComposition : IFeatureComposition { }
}
