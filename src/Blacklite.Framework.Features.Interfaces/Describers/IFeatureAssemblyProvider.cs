using System;
using System.Collections.Generic;
using System.Reflection;

namespace Blacklite.Framework.Features.Describers
{
    public interface IFeatureAssemblyProvider
    {
        IEnumerable<Assembly> Assemblies { get; }
    }
}
