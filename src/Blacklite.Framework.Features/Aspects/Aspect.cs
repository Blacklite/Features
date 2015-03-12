using Blacklite.Framework.Features.OptionModel;
using System;

namespace Blacklite.Framework.Features.Aspects
{
    public abstract class Aspect : IAspect
    {

    }

    public abstract class Aspect<TOptions> : Aspect, IAspect<TOptions>
        where TOptions : class, new()
    {
        public TOptions Options { get; }

        public Aspect(IAspectOptions<TOptions> _optionsContainer)
        {
            Options = _optionsContainer.Options;
        }
    }
}
