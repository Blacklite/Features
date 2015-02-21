using Blacklite.Framework.Features.OptionModel;
using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public abstract partial class Feature
    {
        /// <summary>
        /// Allows a feature to be always off
        /// </summary>
        public class AlwaysOff : IFeature
        {
            public bool IsEnabled { get { return false; } }
        }

        public class AlwaysOff<TOptions> : AlwaysOff, IFeature<TOptions>
            where TOptions : class, new()
        {
            public TOptions Options { get; }

            protected AlwaysOff(IAspectOptions<TOptions> _optionsContainer)
            {
                Options = _optionsContainer.Options;
            }
        }
    }
}
