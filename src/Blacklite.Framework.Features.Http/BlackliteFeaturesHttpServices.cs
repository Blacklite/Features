using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Blacklite.Framework.Features.OptionsModel;
using Blacklite.Framework.Features.Repositories;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Http
{
    public static class BlackliteFeaturesHttpServices
    {
        internal static IEnumerable<IServiceDescriptor> GetFeaturesHttp(IServiceCollection services, IConfiguration configuration = null)
        {
            var describe = new ServiceDescriber(configuration);

            yield return describe.Singleton<IClaimUidExtractor, DefaultClaimUidExtractor>();
            yield return describe.Singleton<AntiForgery, AntiForgery>();
            yield return describe.Singleton<IAntiForgeryAdditionalDataProvider, DefaultAntiForgeryAdditionalDataProvider>();

            yield return describe.Scoped(typeof(IScopedInstance<>), typeof(ScopedInstance<>));
        }
    }
}
