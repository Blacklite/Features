using System;
using System.Collections.Generic;
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
