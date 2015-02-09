using System;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.OptionModel
{
    public interface IFeatureConfigureOptions
    {
        int Priority { get; }
        void Configure(object options);
    }

    public interface IFeatureConfigureOptions<in TOptions> where TOptions : class, new()
    {
        int Priority { get; }
        void Configure(TOptions options);
    }
}