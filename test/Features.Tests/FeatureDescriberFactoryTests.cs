using Blacklite.Framework.Features;
using Blacklite.Framework.Multitenancy.Features;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace Features.Tests
{
    public class FeatureDescriberFactoryTests
    {
        [RequiredFeature(typeof(ScopedFeature))]
        [RequiredFeature(typeof(SingletonFeature))]
        [RequiredFeature(typeof(RealObservableFeature))]
        class TransientFeature : Feature
        {
            public TransientFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(SingletonFeature))]
        [RequiredFeature(typeof(RealObservableFeature))]
        class ScopedFeature : Feature
        {
            public ScopedFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(RealObservableFeature))]
        class SingletonFeature : Feature
        {
            public SingletonFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        class RealObservableFeature : ObservableFeature
        {
            public RealObservableFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(SingletonFeature))]
        class RealObservableFeature2 : ObservableFeature
        {
            public RealObservableFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidScopedFeature : Feature
        {
            public InvalidScopedFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(ScopedFeature))]
        class InvalidSingletonFeature : Feature
        {
            public InvalidSingletonFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidSingletonFeature2 : Feature
        {
            public InvalidSingletonFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(ScopedFeature))]
        class InvalidRealObservableFeature : ObservableFeature
        {
            public InvalidRealObservableFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidRealObservableFeature2 : ObservableFeature
        {
            public InvalidRealObservableFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidTransientRealObservableFeature2 : ObservableFeature
        {
            public InvalidTransientRealObservableFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidScopedRealObservableFeature2 : ObservableFeature
        {
            public InvalidScopedRealObservableFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [Fact]
        public void TransientNeverThrowsForInvalidDescriptor()
        {
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddScoped<InvalidScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<InvalidSingletonFeature>();
            servicesCollection.AddSingleton<InvalidSingletonFeature2>();
            servicesCollection.AddSingleton<RealObservableFeature>();
            servicesCollection.AddSingleton<RealObservableFeature2>();
            servicesCollection.AddSingleton<InvalidRealObservableFeature>();
            servicesCollection.AddSingleton<InvalidRealObservableFeature2>();
            servicesCollection.AddTransient<InvalidTransientRealObservableFeature2>();
            servicesCollection.AddScoped<InvalidScopedRealObservableFeature2>();

            var describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();
            var describer = describers.First(x => x.FeatureType == typeof(TransientFeature));
        }

        [Fact]
        public void ScopedThrowsForInvalidDescriptor()
        {
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableFeature>();
            servicesCollection.AddSingleton<RealObservableFeature2>();
            servicesCollection.AddScoped<InvalidScopedFeature>();
            var describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            var describer = describers.First(x => x.FeatureType == typeof(ScopedFeature));

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidScopedFeature)));
        }

        [Fact]
        public void SingletonThrowsForInvalidDescriptor()
        {
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableFeature>();
            servicesCollection.AddSingleton<RealObservableFeature2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature>();
            var describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            var describer = describers.First(x => x.FeatureType == typeof(SingletonFeature));

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidSingletonFeature)));

            servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableFeature>();
            servicesCollection.AddSingleton<RealObservableFeature2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature2>();
            describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidSingletonFeature2)));
        }

        [Fact]
        public void ObservableThrowsForInvalidDescriptor()
        {
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableFeature>();
            servicesCollection.AddSingleton<RealObservableFeature2>();
            servicesCollection.AddSingleton<InvalidRealObservableFeature>();
            var describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            var describer = describers.First(x => x.FeatureType == typeof(RealObservableFeature));
            FeatureDescriberFactory.ValidateDescriber(describer);

            describer = describers.First(x => x.FeatureType == typeof(RealObservableFeature2));
            FeatureDescriberFactory.ValidateDescriber(describer);

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidRealObservableFeature)));

            servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableFeature>();
            servicesCollection.AddSingleton<RealObservableFeature2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature2>();
            servicesCollection.AddSingleton<InvalidRealObservableFeature2>();
            describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidRealObservableFeature2)));

            servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableFeature>();
            servicesCollection.AddSingleton<RealObservableFeature2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature2>();
            servicesCollection.AddTransient<InvalidTransientRealObservableFeature2>();
            describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidTransientRealObservableFeature2)));

            servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableFeature>();
            servicesCollection.AddSingleton<RealObservableFeature2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature2>();
            servicesCollection.AddScoped<InvalidScopedRealObservableFeature2>();
            describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidScopedRealObservableFeature2)));
        }

        [RequiredFeature(typeof(SingletonFeature))]
        class TenantFeature : Feature
        {
            public TenantFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(TenantFeature))]
        class InvalidTenantFeature : Feature
        {
            public InvalidTenantFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(SingletonFeature))]
        class ApplicationFeature : Feature
        {
            public ApplicationFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(ApplicationFeature))]
        class InvalidApplicationFeature : Feature
        {
            public InvalidApplicationFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [Fact]
        public void SingletonTenantThrowsForInvalidDescriptor()
        {
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddTenantOnlySingleton<TenantFeature>();
            servicesCollection.AddSingleton<InvalidTenantFeature>();

            var describers = new MultitenancyFeatureDescriberFactory().Create(servicesCollection).Cast<MultitenancyFeatureDescriber>();
            var describer = describers.First(x => x.FeatureType == typeof(SingletonFeature));

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidTenantFeature)));
        }

        [Fact]
        public void SingletonApplicationThrowsForInvalidDescriptor()
        {
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddApplicationOnlySingleton<ApplicationFeature>();
            servicesCollection.AddSingleton<InvalidApplicationFeature>();

            var describers = new MultitenancyFeatureDescriberFactory().Create(servicesCollection).Cast<MultitenancyFeatureDescriber>();
            var describer = describers.First(x => x.FeatureType == typeof(SingletonFeature));

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidApplicationFeature)));
        }
    }
}