using Blacklite.Framework.Features.OptionModel;
using System;

namespace Blacklite.Framework.Features.Traits
{
    public abstract partial class Trait : ITrait
    {
        public virtual bool IsEnabled { get; set; } = true;
    }

    public abstract class Trait<TOptions> : Trait, ITrait<TOptions>
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
