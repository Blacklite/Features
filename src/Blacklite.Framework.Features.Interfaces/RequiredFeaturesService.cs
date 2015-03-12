using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    public interface IRequiredFeaturesService
    {
        IValidateFeatureService ValidateFeaturesAreInTheCorrectState(Type type);
    }
}
