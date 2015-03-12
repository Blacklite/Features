using System;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.OptionModel
{
    public interface IAspectOptions { }

    public interface IAspectOptions<out TOptions> : IAspectOptions
        where TOptions : class, new()
    {
        TOptions Options { get; }
    }
}
