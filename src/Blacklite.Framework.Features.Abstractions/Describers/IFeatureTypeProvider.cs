using System;
using System.Collections.Generic;
using System.Reflection;

namespace Blacklite.Framework.Features.Describers
{
    public interface IFeatureTypeProvider
    {
        IEnumerable<TypeInfo> FeatureTypes { get; }
    }
}
