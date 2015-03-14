using System;
using System.Collections.Generic;
using Microsoft.Framework.DependencyInjection;
using System.Linq;
using Blacklite.Framework.Features.OptionsModel;
using System.ComponentModel.DataAnnotations;

namespace Blacklite.Framework.Features.OptionsModel
{
    public interface IFeatureOptions
    {
        void SetOptions(object options);
    }

    public interface IFeatureOptions<out TOptions>
        where TOptions : class, new()
    {
        TOptions Options { get; }
    }
}
