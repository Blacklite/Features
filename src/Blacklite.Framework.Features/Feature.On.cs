using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public abstract partial class Feature
    {
        /// <summary>
        /// Allows a feature to be always on
        /// </summary>
        public class AlwaysOn : IFeature
        {
            public bool IsEnabled { get { return true; } }
        }
    }
}
