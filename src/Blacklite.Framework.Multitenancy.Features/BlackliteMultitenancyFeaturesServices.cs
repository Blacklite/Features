using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Observables;
using Blacklite.Framework.Multitenancy.Features;
using Blacklite.Framework.Multitenancy.Features.Describers;
using Blacklite.Framework.Multitenancy.Features.Observables;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    public static class BlackliteMultitenancyFeaturesServices
    {
        internal static IEnumerable<IServiceDescriptor> GetMultitenancyFeatures(IConfiguration configuration = null)
        {
            var describe = new ServiceDescriber(configuration);

            yield return describe.Singleton<IFeatureDescriberFactory, MultitenancyFeatureDescriberFactory>();
            yield return describe.Singleton<IApplicationOnlyFeatureSubjectFactory, ApplicationOnlyFeatureSubjectFactory>();
            yield return describe.Singleton<ITenantOnlyFeatureSubjectFactory, TenantOnlyFeatureSubjectFactory>();
            yield return describe.Singleton<IFeatureSubjectFactory, MultitenancyCompositeFeatureSubjectFactory>();
        }
    }
}
