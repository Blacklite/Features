using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    public interface IFeatureDescriberProvider
    {
        IReadOnlyDictionary<Type, IFeatureDescriber> Describers { get; }
    }
}
