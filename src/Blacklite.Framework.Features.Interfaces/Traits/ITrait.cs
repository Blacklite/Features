using System;
using System.Collections.Generic;
using Microsoft.Framework.DependencyInjection;
using System.Linq;
using Blacklite.Framework.Features.OptionModel;
using System.ComponentModel.DataAnnotations;
using Blacklite.Framework.Features.Aspects;

namespace Blacklite.Framework.Features.Traits
{
    public interface ITrait : IAspect
    {
        [Display(Name = "On")]
        bool IsEnabled { get; }
    }

    public interface ITraitOptions
    {
        void SetOptions(object options);
    }

    public interface ITrait<TOptions> : ITrait, ITraitOptions
        where TOptions : class, new()
    {
        TOptions Options { get; }
        void SetOptions(TOptions options);
    }
}
