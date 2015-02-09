using System;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.OptionModel
{
    public interface IFeatureOptions<out TOptions> where TOptions : class, new()
    {
        TOptions Options { get; }
    }
}