using Blacklite.Framework.Features.OptionsModel;
using System;

namespace Blacklite.Framework.Features
{
    public abstract partial class Switch : ISwitch
    {
        public virtual bool IsEnabled { get; set; } = true;
    }

    public abstract class Switch<TOptions> : Switch, ISwitch<TOptions>
        where TOptions : class, new()
    {
        public TOptions Options { get; private set; }

        void IFeatureOptions.SetOptions(object options)
        {
            Options = (TOptions)options;
        }
    }
}
