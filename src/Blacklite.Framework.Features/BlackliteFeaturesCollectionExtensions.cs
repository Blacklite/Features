using Blacklite;
using Blacklite.Framework;
using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Describers;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Framework.DependencyInjection
{
    public static class BlackliteFeaturesCollectionExtensions
    {
        private static Type GetImplementationType(this ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
            {
                return descriptor.ImplementationType;
            }
            else if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance.GetType();
            }
            else if (descriptor.ImplementationFactory != null)
            {
                var typeArguments = descriptor.ImplementationFactory.GetType().GenericTypeArguments;
                return typeArguments[1];
            }

            throw new ArgumentException("Resources.FormatNoImplementation(ServiceType)");
        }

        // TODO REMOVE WHEN NOT NEEDED
        private static void TryAddEnumerable(
            [NotNull] this IServiceCollection services,
            [NotNull] ServiceDescriptor descriptor)
        {
            var implementationType = descriptor.GetImplementationType();

            if (implementationType == typeof(object) ||
                implementationType == descriptor.ServiceType)
            {
                throw new ArgumentException(nameof(descriptor));
            }

            if (!services.Any(d =>
                d.ServiceType == descriptor.ServiceType &&
                d.GetImplementationType() == implementationType))
            {
                services.Add(descriptor);
            }
        }

        private static void TryAddEnumerable(
            [NotNull] this IServiceCollection services,
            [NotNull] IEnumerable<ServiceDescriptor> descriptors)
        {
            foreach (var d in descriptors)
            {
                services.TryAddEnumerable(d);
            }
        }

        public static IServiceCollection AddFeatures([NotNull] this IServiceCollection services)
        {
            services.TryAdd(BlackliteFeaturesServices.GetFeatures(services));
            services.TryAddEnumerable(BlackliteFeaturesServices.GetEnumerableFeatures(services));
            return services;
        }

        public static IServiceCollection AddFeaturesConfiguration([NotNull] this IServiceCollection services, IConfiguration configuration, Func<IFeatureDescriber, bool> predicate = null)
        {
            services.TryAddEnumerable(BlackliteFeaturesServices.GetFeaturesConfiguration(services, configuration, predicate));
            return services;
        }
    }
}
