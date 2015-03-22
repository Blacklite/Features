using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using Blacklite.Framework.Features.Describers;

namespace Blacklite.Framework.Features
{
    public interface IFeatureManager
    {
        bool TrySaveFeature(IFeatureDescriber describer, IFeature feature);
    }
}
