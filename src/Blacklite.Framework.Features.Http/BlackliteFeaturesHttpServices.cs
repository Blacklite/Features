using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Blacklite.Framework.Features.OptionsModel;
using Blacklite.Framework.Features.Repositories;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Http
{
    public static class BlackliteFeaturesHttpServices
    {
        internal static IEnumerable<ServiceDescriptor> GetFeaturesHttp(IServiceCollection services)
        {
            return Enumerable.Empty<ServiceDescriptor>();
        }
    }
}
