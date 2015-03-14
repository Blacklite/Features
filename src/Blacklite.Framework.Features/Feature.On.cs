using Blacklite.Framework.Features.OptionModel;
using Blacklite.Framework.Features.Traits;
using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public sealed partial class Feature
    {
        /// <summary>
        /// Allows a feature to be always on
        /// </summary>
        public class AlwaysOn : ITrait
        {
            public bool IsEnabled { get { return true; } }
        }

        public class AlwaysOn<TOptions> : AlwaysOn, ITrait<TOptions>
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
