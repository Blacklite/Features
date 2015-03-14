using Blacklite.Framework.Features.OptionsModel;
using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public abstract partial class Feature
    {
        /// <summary>
        /// Allows a feature to be always on
        /// </summary>
        public class AlwaysOn : ISwitch
        {
            public bool IsEnabled { get { return true; } }
        }

        public class AlwaysOn<TOptions> : AlwaysOn, ISwitch<TOptions>
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
