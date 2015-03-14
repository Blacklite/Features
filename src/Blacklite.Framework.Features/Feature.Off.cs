using Blacklite.Framework.Features.OptionsModel;
using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public abstract partial class Feature
    {
        /// <summary>
        /// Allows a feature to be always off
        /// </summary>
        public class AlwaysOff : ISwitch
        {
            public bool IsEnabled { get { return false; } }
        }

        public class AlwaysOff<TOptions> : AlwaysOff, ISwitch<TOptions>
            where TOptions : class, new()
        {
            public TOptions Options { get; private set; }

            void IFeatureOptions.SetOptions(object options)
            {
                Options = (TOptions)options;
            }
        }
    }
}
