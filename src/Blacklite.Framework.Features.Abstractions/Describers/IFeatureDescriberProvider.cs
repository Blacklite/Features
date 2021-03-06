﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

namespace Blacklite.Framework.Features.Describers
{
    public interface IFeatureDescriberProvider
    {
        IReadOnlyDictionary<Type, IFeatureDescriber> Describers { get; }
    }
}
