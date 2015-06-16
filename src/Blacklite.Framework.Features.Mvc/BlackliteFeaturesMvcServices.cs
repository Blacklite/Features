using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Blacklite.Framework.Features.OptionsModel;
using Blacklite.Framework.Features.Repositories;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Mvc
{
    public static class BlackliteFeaturesMvcServices
    {
        internal static IEnumerable<ServiceDescriptor> GetFeaturesMvc(IServiceCollection services)
        {
            return Enumerable.Empty<ServiceDescriptor>();
        }
    }
}
