using Blacklite.Framework.Features;
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
        [RequiredFeature(typeof(RealObservableSwitch))]
        class TransientFeature : Switch
        {
        }

        [RequiredFeature(typeof(SingletonFeature))]
        [RequiredFeature(typeof(RealObservableSwitch))]
        class ScopedFeature : Switch
        {
        }

        [RequiredFeature(typeof(RealObservableSwitch))]
        class SingletonFeature : Switch
        {
        }

        class RealObservableSwitch : ObservableSwitch
        {
        }

        [RequiredFeature(typeof(SingletonFeature))]
        class RealObservableSwitch2 : ObservableSwitch
        {
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidScopedFeature : Switch
        {
        }

        [RequiredFeature(typeof(ScopedFeature))]
        class InvalidSingletonFeature : Switch
        {
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidSingletonFeature2 : Switch
        {
        }

        [RequiredFeature(typeof(ScopedFeature))]
        class InvalidRealObservableSwitch : ObservableSwitch
        {
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidRealObservableSwitch2 : ObservableSwitch
        {
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidTransientRealObservableSwitch2 : ObservableSwitch
        {
        }

        [RequiredFeature(typeof(TransientFeature))]
        class InvalidScopedRealObservableSwitch2 : ObservableSwitch
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
            servicesCollection.AddSingleton<RealObservableSwitch>();
            servicesCollection.AddSingleton<RealObservableSwitch2>();
            servicesCollection.AddSingleton<InvalidRealObservableSwitch>();
            servicesCollection.AddSingleton<InvalidRealObservableSwitch2>();
            servicesCollection.AddTransient<InvalidTransientRealObservableSwitch2>();
            servicesCollection.AddScoped<InvalidScopedRealObservableSwitch2>();

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
            servicesCollection.AddSingleton<RealObservableSwitch>();
            servicesCollection.AddSingleton<RealObservableSwitch2>();
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
            servicesCollection.AddSingleton<RealObservableSwitch>();
            servicesCollection.AddSingleton<RealObservableSwitch2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature>();
            var describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            var describer = describers.First(x => x.FeatureType == typeof(SingletonFeature));

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidSingletonFeature)));

            servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableSwitch>();
            servicesCollection.AddSingleton<RealObservableSwitch2>();
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
            servicesCollection.AddSingleton<RealObservableSwitch>();
            servicesCollection.AddSingleton<RealObservableSwitch2>();
            servicesCollection.AddSingleton<InvalidRealObservableSwitch>();
            var describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            var describer = describers.First(x => x.FeatureType == typeof(RealObservableSwitch));
            FeatureDescriberFactory.ValidateDescriber(describer);

            describer = describers.First(x => x.FeatureType == typeof(RealObservableSwitch2));
            FeatureDescriberFactory.ValidateDescriber(describer);

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidRealObservableSwitch)));

            servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableSwitch>();
            servicesCollection.AddSingleton<RealObservableSwitch2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature2>();
            servicesCollection.AddSingleton<InvalidRealObservableSwitch2>();
            describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidRealObservableSwitch2)));

            servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableSwitch>();
            servicesCollection.AddSingleton<RealObservableSwitch2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature2>();
            servicesCollection.AddTransient<InvalidTransientRealObservableSwitch2>();
            describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidTransientRealObservableSwitch2)));

            servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<RealObservableSwitch>();
            servicesCollection.AddSingleton<RealObservableSwitch2>();
            servicesCollection.AddSingleton<InvalidSingletonFeature2>();
            servicesCollection.AddScoped<InvalidScopedRealObservableSwitch2>();
            describers = new FeatureDescriberFactory().Create(servicesCollection).Cast<FeatureDescriber>();

            Assert.Throws<NotSupportedException>(() => describers.First(x => x.FeatureType == typeof(InvalidScopedRealObservableSwitch2)));
        }
    }
}
