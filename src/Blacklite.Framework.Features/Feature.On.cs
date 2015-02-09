using Blacklite.Framework.Features.OptionModel;
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

        public class AlwaysOn<TOptions> : AlwaysOn, IFeature<TOptions>
            where TOptions : class, new()
        {
            public TOptions Options { get; }

            protected AlwaysOn(IFeatureOptions<TOptions> _optionsContainer)
            {
                Options = _optionsContainer.Options;
            }
        }
    }
}
