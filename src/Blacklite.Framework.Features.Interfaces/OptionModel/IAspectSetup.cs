using Blacklite.Framework.Features.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.OptionModel
{
    public interface IAspectSetup
    {
        int Priority { get; }
        bool IsApplicableTo(IFeatureDescriber describer);
        T Configure<T>(T aspect);
    }

    public interface IAspectSetup<in TAspect> where TAspect : class, new()
    {
        int Priority { get; }
        IAspect Configure(TAspect aspect);
    }
}
