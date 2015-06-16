using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features.Composition
{
    public interface IFeatureComposition
    {
        int Priority { get; }
        bool IsApplicableTo(IFeatureDescriber describer);
        T Configure<T>(T feature, IFeatureDescriber describer, IFeatureFactory factory);
    }

    public interface IPreFeatureComposition : IFeatureComposition { }

    public interface IPostFeatureComposition : IFeatureComposition { }
}
