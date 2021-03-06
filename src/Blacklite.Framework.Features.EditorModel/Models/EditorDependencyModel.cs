﻿using Blacklite.Framework.Features.Describers;
using System;

namespace Blacklite.Framework.Features.Editors.Models
{
    public class EditorDependencyModel
    {
        public EditorDependencyModel(IFeatureDescriber describer, bool isEnabled)
        {
            Feature = describer;
            IsEnabled = isEnabled;
        }

        public IFeatureDescriber Feature { get; }
        public bool IsEnabled { get; }
    }
}
