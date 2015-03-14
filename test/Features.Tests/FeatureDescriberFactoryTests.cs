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
        }

        [RequiredFeature(typeof(SingletonFeature))]
        [RequiredFeature(typeof(RealObservableTrait))]
        class ScopedFeature : Trait
        {
        }

        [RequiredFeature(typeof(RealObservableTrait))]
        class SingletonFeature : Trait
        {
        }

        class RealObservableTrait : ObservableTrait
        {
        }

        [RequiredFeature(typeof(SingletonFeature))]
        class RealObservableTrait2 : ObservableTrait
        {
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidScopedFeature : Trait
        {
        }

        [RequiredFeature(typeof(ScopedFeature))]
        class InvalidSingletonFeature : Trait
        {
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidSingletonFeature2 : Trait
        {
        }

        [RequiredFeature(typeof(ScopedFeature))]
        class InvalidRealObservableTrait : ObservableTrait
        {
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidRealObservableTrait2 : ObservableTrait
        {
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidTransientRealObservableTrait2 : ObservableTrait
        {
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidScopedRealObservableTrait2 : ObservableTrait
        {
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
