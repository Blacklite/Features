using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public abstract partial class Feature
    {
        /// <summary>
        /// Allows a feature to be always off
        /// </summary>
        public class AlwaysOff : Feature
        {
            public override bool IsEnabled { get { return false; } }
        }
    }
}
