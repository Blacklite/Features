using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Observables;
using Blacklite.Framework.Multitenancy.Features;
using Blacklite.Framework.Multitenancy.Features.Describers;
using Blacklite.Framework.Multitenancy.Features.Observables;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    public static class BlackliteMultitenancyFeaturesServices
    {
        internal static IEnumerable<ServiceDescriptor> GetMultitenancyFeatures()
        {
            yield return ServiceDescriptor.Singleton<IFeatureDescriberFactory, MultitenancyFeatureDescriberFactory>();
            yield return ServiceDescriptor.Singleton<IApplicationOnlyFeatureSubjectFactory, ApplicationOnlyFeatureSubjectFactory>();
            yield return ServiceDescriptor.Singleton<ITenantOnlyFeatureSubjectFactory, TenantOnlyFeatureSubjectFactory>();
            yield return ServiceDescriptor.Singleton<IFeatureSubjectFactory, MultitenancyCompositeFeatureSubjectFactory>();
        }
    }
}
