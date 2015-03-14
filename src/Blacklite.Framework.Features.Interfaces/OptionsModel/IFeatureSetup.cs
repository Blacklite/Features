using System;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.OptionsModel
{
    public interface IFeatureSetup
    {
        int Priority { get; }
        bool IsApplicableTo(IFeatureDescriber describer);
        T Configure<T>(T aspect);
    }

    public interface IFeatureSetup<in TAspect>
        where TAspect : class, new()
    {
        int Priority { get; }
        IFeature Configure(TAspect aspect);
    }
}
