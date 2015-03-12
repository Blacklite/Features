using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Traits;
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
        [RequiredFeature(typeof(RealObservableTrait))]
        class TransientFeature : Trait
        {
            public TransientFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(SingletonFeature))]
        [RequiredFeature(typeof(RealObservableTrait))]
        class ScopedFeature : Trait
        {
            public ScopedFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(RealObservableTrait))]
        class SingletonFeature : Trait
        {
            public SingletonFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        class RealObservableTrait : ObservableTrait
        {
            public RealObservableTrait(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(SingletonFeature))]
        class RealObservableTrait2 : ObservableTrait
        {
            public RealObservableTrait2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidScopedFeature : Trait
        {
            public InvalidScopedFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(ScopedFeature))]
        class InvalidSingletonFeature : Trait
        {
            public InvalidSingletonFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidSingletonFeature2 : Trait
        {
            public InvalidSingletonFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(ScopedFeature))]
        class InvalidRealObservableTrait : ObservableTrait
        {
            public InvalidRealObservableTrait(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidRealObservableTrait2 : ObservableTrait
        {
            public InvalidRealObservableTrait2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidTransientRealObservableTrait2 : ObservableTrait
        {
            public InvalidTransientRealObservableTrait2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidScopedRealObservableTrait2 : ObservableTrait
        {
            public InvalidScopedRealObservableTrait2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
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
            servicesCollection.AddSingleton<RealObservableTrait>();
            servicesCollection.AddSingleton<RealObservableTrait2>();
            servicesCollection.AddSingleton<InvalidRealObservableTrait>();
            servicesCollection.AddSingleton<InvalidRealObservableTrait2>();
            servicesCollection.AddTransient<InvalidTransientRealObservableTrait2>();
            servicesCollection.AddScoped<InvalidScopedRealObservableTrait2>();

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
            servicesCollection.AddSingleton<RealObservableTrait>();
            servicesCollection.AddSingleton<RealObservableTrait2>();
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
            servicesCollection.AddSingleton<RealObservableTrait>();
            servicesCollection.AddSingleton<RealObservableTrait2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature>();
            var describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            var describer = describers.First(x => x.FeatureType == typeof(SingletonFeature));

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidSingletonFeature)));

            servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableTrait>();
            servicesCollection.AddSingleton<RealObservableTrait2>();
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
            servicesCollection.AddSingleton<RealObservableTrait>();
            servicesCollection.AddSingleton<RealObservableTrait2>();
            servicesCollection.AddSingleton<InvalidRealObservableTrait>();
            var describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            var describer = describers.First(x => x.FeatureType == typeof(RealObservableTrait));
            FeatureDescriberFactory.ValidateDescriber(describer);

            describer = describers.First(x => x.FeatureType == typeof(RealObservableTrait2));
            FeatureDescriberFactory.ValidateDescriber(describer);

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidRealObservableTrait)));

            servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableTrait>();
            servicesCollection.AddSingleton<RealObservableTrait2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature2>();
            servicesCollection.AddSingleton<InvalidRealObservableTrait2>();
            describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidRealObservableTrait2)));

            servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableTrait>();
            servicesCollection.AddSingleton<RealObservableTrait2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature2>();
            servicesCollection.AddTransient<InvalidTransientRealObservableTrait2>();
            describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidTransientRealObservableTrait2)));

            servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableTrait>();
            servicesCollection.AddSingleton<RealObservableTrait2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature2>();
            servicesCollection.AddScoped<InvalidScopedRealObservableTrait2>();
            describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidScopedRealObservableTrait2)));
        }
    }
}
