using System;
using System.Collections.Generic;
using Microsoft.Framework.DependencyInjection;
using System.Linq;
using Blacklite.Framework.Features.OptionModel;
using System.ComponentModel.DataAnnotations;

namespace Blacklite.Framework.Features
{
    public interface IAspect { }

    public interface IAspect<TOptions> : IAspect, IAspectOptions
        where TOptions : class, new()
    {
        TOptions Options { get; }
    }

    public interface IFeature : IAspect
    {
        [Display(Name = "On")]
        bool IsEnabled { get; }
    }

    public interface IFeature<TOptions> : IFeature, IAspect<TOptions>
        where TOptions : class, new()
    {
    }
}
