using Blacklite.Framework.Features;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Multitenancy.Features
{
    class MultitenancyFeatureDescriber : FeatureDescriber
    {
        public MultitenancyFeatureDescriber(IServiceDescriptor descriptor)
            : base(descriptor)
        {
            IsApplicationScoped = descriptor is ApplicationOnlyServiceDescriptor;
            IsTenantScoped = descriptor is TenantOnlyServiceDescriptor;
        }

        public bool IsTenantScoped { get; }

        public bool IsApplicationScoped { get; }
    }
}
