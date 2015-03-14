using System;
using System.Collections.Generic;
using Microsoft.Framework.DependencyInjection;
using System.Linq;
using Blacklite.Framework.Features.OptionModel;
using System.ComponentModel.DataAnnotations;
using Blacklite.Framework.Features.Aspects;

namespace Blacklite.Framework.Features
{
    public interface IAspectOptions { }
    public interface IAspectOptions<TOptions> : IAspectOptions
        where TOptions : class, new()
    {
        TOptions Options { get; }
    }
}
