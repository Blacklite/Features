using System;
using System.Collections.Generic;
using Blacklite.Framework.Features.OptionsModel;
using System.ComponentModel.DataAnnotations;

namespace Blacklite.Framework.Features
{
    public interface ISwitch : IFeature
    {
        [Display(Name = "On")]
        bool IsEnabled { get; }
    }

    public interface ISwitch<TOptions> : ISwitch, IFeatureOptions
        where TOptions : class, new()
    {
        TOptions Options { get; }
    }
}
