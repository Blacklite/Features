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

    public interface IOptionsFeatureComposer : IFeatureComposition { }

    public interface IRequiredFeatureComposer : IFeatureComposition { }
}
