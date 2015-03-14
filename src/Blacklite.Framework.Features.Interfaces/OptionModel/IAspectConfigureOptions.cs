using System;

namespace Blacklite.Framework.Features.OptionModel
{
    public interface IAspectConfigureOptions
    {
        int Priority { get; }
        void Configure(object options);
    }

    public interface IAspectConfigureOptions<in TOptions> where TOptions : class, new()
    {
        int Priority { get; }
        void Configure(TOptions options);
    }
}
