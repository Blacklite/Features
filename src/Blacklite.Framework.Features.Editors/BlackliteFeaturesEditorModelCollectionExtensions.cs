using Blacklite;
using Blacklite.Framework;
using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Editors;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Framework.DependencyInjection
{
    public static class BlackliteFeaturesEditorModelCollectionExtensions
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

        public static IServiceCollection AddFeatureEditorModel([NotNull] this IServiceCollection services)
        {
            services.AddFeatures();

            services.TryAdd(BlackliteFeaturesEditorModelServices.GetFeatureEditorModel(services));
            services.TryAdd(OptionsServiceCollectionExtensions.AddOptions(services));
            return services;
        }
    }
}
