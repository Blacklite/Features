using Blacklite.Framework.Features.OptionModel;
using Blacklite.Framework.Features.Traits;
using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public sealed partial class Feature
    {
        /// <summary>
        /// Allows a feature to be always off
        /// </summary>
        public class AlwaysOff : ITrait
        {
            public bool IsEnabled { get { return false; } }
        }

        public class AlwaysOff<TOptions> : AlwaysOff, ITrait<TOptions>
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
