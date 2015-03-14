using Blacklite.Framework.Features.Aspects;
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
            public TOptions Options { get; private set; }

            void ITraitOptions.SetOptions(object options)
            {
                Options = (TOptions)options;
            }

            void ITrait<TOptions>.SetOptions(TOptions options)
            {
                Options = options;
            }
        }
    }
}
