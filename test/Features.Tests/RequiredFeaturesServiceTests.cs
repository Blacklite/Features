using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Microsoft.Framework.DependencyInjection;
using Moq;
using System;
using Xunit;

namespace Features.Tests
{
    public class RequiredFeaturesServiceTests
    {
        class TransientFeature : Switch
        {
        }

        class ScopedFeature : Switch
        {
        }

        class SingletonFeature : Switch
        {
        }

        class RealObservableSwitch : ObservableSwitch
        {
        }

        [RequiredFeature(typeof(RealObservableSwitch))]
        [RequiredFeature(typeof(RealObservableSwitch2), true)]
        [RequiredFeature(typeof(SingletonFeature))]
        [RequiredFeature(typeof(ScopedFeature), true)]
        [RequiredFeature(typeof(TransientFeature), true)]
        class TransientFeature2 : Switch
        {
        }

        [RequiredFeature(typeof(RealObservableSwitch), true)]
        [RequiredFeature(typeof(SingletonFeature2))]
        [RequiredFeature(typeof(ScopedFeature), true)]
        class ScopedFeature2 : Switch
        {
        }

        [RequiredFeature(typeof(RealObservableSwitch))]
        [RequiredFeature(typeof(RealObservableSwitch2), true)]
        class SingletonFeature2 : Switch
        {
        }

        [RequiredFeature(typeof(SingletonFeature), true)]
        class RealObservableSwitch2 : ObservableSwitch
        {
        }

        class TransientObservableSwitch : ObservableSwitch
        {
        }

        class ScopedObservableSwitch : ObservableSwitch
        {
        }

        [Fact]
        public void ReturnsTrueByDefault()
        {

            var servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddTransient<TransientFeature2>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddScoped<ScopedFeature2>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<SingletonFeature2>();
            servicesCollection.AddSingleton<RealObservableSwitch>();
            servicesCollection.AddSingleton<RealObservableSwitch2>();

            var featureDescriberProvider = new FeatureDescriberProvider(
                new FeatureServicesCollection(servicesCollection), new FeatureDescriberFactory());

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceProvider = serviceProviderMock.Object;

            var featureFactoryMock = new Mock<IFeatureFactory>();
            featureFactoryMock.Setup(x => x.GetFeature<TransientFeature>()).Returns(() => new TransientFeature());
            featureFactoryMock.Setup(x => x.GetFeature<TransientFeature2>()).Returns(() => new TransientFeature2());
            featureFactoryMock.Setup(x => x.GetFeature<ScopedFeature>()).Returns(new ScopedFeature());
            featureFactoryMock.Setup(x => x.GetFeature<ScopedFeature2>()).Returns(new ScopedFeature2());
            featureFactoryMock.Setup(x => x.GetFeature<SingletonFeature>()).Returns(new SingletonFeature());
            featureFactoryMock.Setup(x => x.GetFeature<SingletonFeature2>()).Returns(new SingletonFeature2());
            featureFactoryMock.Setup(x => x.GetFeature<RealObservableSwitch>()).Returns(new RealObservableSwitch());
            featureFactoryMock.Setup(x => x.GetFeature<RealObservableSwitch2>()).Returns(new RealObservableSwitch2());
            var featureFactory = featureFactoryMock.Object;

            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<TransientFeature>))).Returns(() => new FeatureImpl< TransientFeature>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<TransientFeature2>))).Returns(() => new FeatureImpl<TransientFeature2>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<ScopedFeature>))).Returns(new FeatureImpl<ScopedFeature>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<ScopedFeature2>))).Returns(new FeatureImpl<ScopedFeature2>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<SingletonFeature>))).Returns(new FeatureImpl<SingletonFeature>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<SingletonFeature2>))).Returns(new FeatureImpl<SingletonFeature2>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<RealObservableSwitch>))).Returns(new FeatureImpl<RealObservableSwitch>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<RealObservableSwitch2>))).Returns(new FeatureImpl<RealObservableSwitch2>(featureFactory));

            var realObservableSwitch = new FeatureImpl<RealObservableSwitch>(featureFactory);
            var realObservableSubject = new FeatureSubject<RealObservableSwitch>(realObservableSwitch, serviceProvider, service);
            serviceProviderMock.Setup(x => x.GetService(typeof(ObservableFeature<RealObservableSwitch>))).Returns(new ObservableFeatureImpl<RealObservableSwitch>(realObservableSubject));

            var realObservableSwitch2 = new FeatureImpl<RealObservableSwitch2>(featureFactory);
            var realObservableSubject2 = new FeatureSubject<RealObservableSwitch2>(realObservableSwitch2, serviceProvider, service);
            serviceProviderMock.Setup(x => x.GetService(typeof(ObservableFeature<RealObservableSwitch2>))).Returns(new ObservableFeatureImpl<RealObservableSwitch2>(realObservableSubject2));

            bool result = false;
            result = service.ValidateRequiredFeatures(typeof(TransientFeature));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(TransientFeature2));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(ScopedFeature));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(ScopedFeature2));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(SingletonFeature));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(SingletonFeature2));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(RealObservableSwitch));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(RealObservableSwitch2));
            Assert.True(result);

            Assert.Equal(8, featureDescriberProvider.Describers.Count);
        }

        [Fact]
        public void HandlesObservables()
        {

            var servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddTransient<TransientFeature2>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddScoped<ScopedFeature2>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<SingletonFeature2>();
            servicesCollection.AddSingleton<RealObservableSwitch>();
            servicesCollection.AddSingleton<RealObservableSwitch2>();

            var featureDescriberProvider = new FeatureDescriberProvider(
                new FeatureServicesCollection(servicesCollection), new FeatureDescriberFactory());

            var featureFactoryMock = new Mock<IFeatureFactory>();
            featureFactoryMock.Setup(x => x.GetFeature<TransientFeature>()).Returns(() => new TransientFeature());
            featureFactoryMock.Setup(x => x.GetFeature<TransientFeature2>()).Returns(() => new TransientFeature2());
            featureFactoryMock.Setup(x => x.GetFeature<ScopedFeature>()).Returns(new ScopedFeature());
            featureFactoryMock.Setup(x => x.GetFeature<ScopedFeature2>()).Returns(new ScopedFeature2());
            featureFactoryMock.Setup(x => x.GetFeature<SingletonFeature>()).Returns(new SingletonFeature());
            featureFactoryMock.Setup(x => x.GetFeature<SingletonFeature2>()).Returns(new SingletonFeature2());
            featureFactoryMock.Setup(x => x.GetFeature<RealObservableSwitch>()).Returns(new RealObservableSwitch());
            featureFactoryMock.Setup(x => x.GetFeature<RealObservableSwitch2>()).Returns(new RealObservableSwitch2());
            var featureFactory = featureFactoryMock.Object;

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceProvider = serviceProviderMock.Object;

            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<TransientFeature>))).Returns(() => new FeatureImpl<TransientFeature>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<TransientFeature2>))).Returns(() => new FeatureImpl<TransientFeature2>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<ScopedFeature>))).Returns(new FeatureImpl<ScopedFeature>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<ScopedFeature2>))).Returns(new FeatureImpl<ScopedFeature2>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<SingletonFeature>))).Returns(new FeatureImpl<SingletonFeature>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<SingletonFeature2>))).Returns(new FeatureImpl<SingletonFeature2>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<RealObservableSwitch>))).Returns(new FeatureImpl<RealObservableSwitch>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<RealObservableSwitch2>))).Returns(new FeatureImpl<RealObservableSwitch2>(featureFactory));

            var realObservableSwitch = new FeatureImpl<RealObservableSwitch>(featureFactory);
            var realObservableSubject = new FeatureSubject<RealObservableSwitch>(realObservableSwitch, serviceProvider, service);
            serviceProviderMock.Setup(x => x.GetService(typeof(ObservableFeature<RealObservableSwitch>))).Returns(new ObservableFeatureImpl<RealObservableSwitch>(realObservableSubject));

            var realObservableSwitch2 = new FeatureImpl<RealObservableSwitch2>(featureFactory);
            var realObservableSubject2 = new FeatureSubject<RealObservableSwitch2>(realObservableSwitch2, serviceProvider, service);
            serviceProviderMock.Setup(x => x.GetService(typeof(ObservableFeature<RealObservableSwitch2>))).Returns(new ObservableFeatureImpl<RealObservableSwitch2>(realObservableSubject2));

            bool result = false;

            result = service.ValidateRequiredFeatures(typeof(TransientFeature2));
            Assert.True(result);

            realObservableSwitch.Value.IsEnabled = false;
            realObservableSubject.OnNext(realObservableSwitch.Value);

            result = service.ValidateRequiredFeatures(typeof(TransientFeature2));
            Assert.False(result);

            realObservableSwitch.Value.IsEnabled = true;
            realObservableSubject.OnNext(realObservableSwitch.Value);

            result = service.ValidateRequiredFeatures(typeof(TransientFeature2));
            Assert.True(result);

        }

        [Fact]
        public void ReturnsFalseWhenAppropriate()
        {

            var servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddTransient<TransientFeature2>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddScoped<ScopedFeature2>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<SingletonFeature2>();
            servicesCollection.AddSingleton<RealObservableSwitch>();
            servicesCollection.AddSingleton<RealObservableSwitch2>();

            var featureDescriberProvider = new FeatureDescriberProvider(
                new FeatureServicesCollection(servicesCollection), new FeatureDescriberFactory());

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceProvider = serviceProviderMock.Object;

            var transientFeature = new TransientFeature();
            transientFeature.IsEnabled = false;

            var featureFactoryMock = new Mock<IFeatureFactory>();
            featureFactoryMock.Setup(x => x.GetFeature<TransientFeature>()).Returns(() => transientFeature);
            featureFactoryMock.Setup(x => x.GetFeature<TransientFeature2>()).Returns(() => new TransientFeature2());
            featureFactoryMock.Setup(x => x.GetFeature<ScopedFeature>()).Returns(new ScopedFeature());
            featureFactoryMock.Setup(x => x.GetFeature<ScopedFeature2>()).Returns(new ScopedFeature2());
            featureFactoryMock.Setup(x => x.GetFeature<SingletonFeature>()).Returns(new SingletonFeature());
            featureFactoryMock.Setup(x => x.GetFeature<SingletonFeature2>()).Returns(new SingletonFeature2());
            featureFactoryMock.Setup(x => x.GetFeature<RealObservableSwitch>()).Returns(new RealObservableSwitch());
            featureFactoryMock.Setup(x => x.GetFeature<RealObservableSwitch2>()).Returns(new RealObservableSwitch2());
            var featureFactory = featureFactoryMock.Object;

            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<TransientFeature>))).Returns(() => new FeatureImpl<TransientFeature>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<TransientFeature2>))).Returns(() => new FeatureImpl<TransientFeature2>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<ScopedFeature>))).Returns(new FeatureImpl<ScopedFeature>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<ScopedFeature2>))).Returns(new FeatureImpl<ScopedFeature2>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<SingletonFeature>))).Returns(new FeatureImpl<SingletonFeature>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<SingletonFeature2>))).Returns(new FeatureImpl<SingletonFeature2>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<RealObservableSwitch>))).Returns(new FeatureImpl<RealObservableSwitch>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<RealObservableSwitch2>))).Returns(new FeatureImpl<RealObservableSwitch2>(featureFactory));

            var realObservableSwitch = new FeatureImpl<RealObservableSwitch>(featureFactory);
            var realObservableSubject = new FeatureSubject<RealObservableSwitch>(realObservableSwitch, serviceProvider, service);
            serviceProviderMock.Setup(x => x.GetService(typeof(ObservableFeature<RealObservableSwitch>))).Returns(new ObservableFeatureImpl<RealObservableSwitch>(realObservableSubject));

            var realObservableSwitch2 = new FeatureImpl<RealObservableSwitch2>(featureFactory);
            var realObservableSubject2 = new FeatureSubject<RealObservableSwitch2>(realObservableSwitch2, serviceProvider, service);
            serviceProviderMock.Setup(x => x.GetService(typeof(ObservableFeature<RealObservableSwitch2>))).Returns(new ObservableFeatureImpl<RealObservableSwitch2>(realObservableSubject2));

            bool result = false;
            result = service.ValidateRequiredFeatures(typeof(TransientFeature));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(TransientFeature2));
            Assert.False(result);

            result = service.ValidateRequiredFeatures(typeof(ScopedFeature));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(ScopedFeature2));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(SingletonFeature));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(SingletonFeature2));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(RealObservableSwitch));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(RealObservableSwitch2));
            Assert.True(result);

            Assert.Equal(8, featureDescriberProvider.Describers.Count);
        }

        [RequiredFeature(typeof(RealObservableSwitch))]
        [RequiredFeature(typeof(RealObservableSwitch2), false)]
        [RequiredFeature(typeof(SingletonFeature), true)]
        [RequiredFeature(typeof(ScopedFeature), false)]
        [RequiredFeature(typeof(TransientFeature), false)]
        class TransientFeature3 : Switch
        {
        }

        [Fact]
        public void ReturnsFalseWhenAppropriate2()
        {
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<TransientFeature>();
            servicesCollection.AddTransient<TransientFeature3>();
            servicesCollection.AddScoped<ScopedFeature>();
            servicesCollection.AddScoped<ScopedFeature2>();
            servicesCollection.AddSingleton<SingletonFeature>();
            servicesCollection.AddSingleton<SingletonFeature2>();
            servicesCollection.AddSingleton<RealObservableSwitch>();
            servicesCollection.AddSingleton<RealObservableSwitch2>();

            var featureDescriberProvider = new FeatureDescriberProvider(
                new FeatureServicesCollection(servicesCollection), new FeatureDescriberFactory());

            var transientFeature = new TransientFeature();
            var scopedFeature = new ScopedFeature();
            var singletonFeature = new SingletonFeature();
            var featureFactoryMock = new Mock<IFeatureFactory>();
            featureFactoryMock.Setup(x => x.GetFeature<TransientFeature>()).Returns(() => transientFeature);
            featureFactoryMock.Setup(x => x.GetFeature<TransientFeature3>()).Returns(() => new TransientFeature3());
            featureFactoryMock.Setup(x => x.GetFeature<ScopedFeature>()).Returns(scopedFeature);
            featureFactoryMock.Setup(x => x.GetFeature<ScopedFeature2>()).Returns(new ScopedFeature2());
            featureFactoryMock.Setup(x => x.GetFeature<SingletonFeature>()).Returns(singletonFeature);
            featureFactoryMock.Setup(x => x.GetFeature<SingletonFeature2>()).Returns(new SingletonFeature2());
            featureFactoryMock.Setup(x => x.GetFeature<RealObservableSwitch>()).Returns(new RealObservableSwitch());
            featureFactoryMock.Setup(x => x.GetFeature<RealObservableSwitch2>()).Returns(new RealObservableSwitch2());
            var featureFactory = featureFactoryMock.Object;

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceProvider = serviceProviderMock.Object;

            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<TransientFeature>))).Returns(() => new FeatureImpl<TransientFeature>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<TransientFeature2>))).Returns(() => new FeatureImpl<TransientFeature2>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<ScopedFeature>))).Returns(new FeatureImpl<ScopedFeature>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<ScopedFeature2>))).Returns(new FeatureImpl<ScopedFeature2>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<SingletonFeature>))).Returns(new FeatureImpl<SingletonFeature>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<SingletonFeature2>))).Returns(new FeatureImpl<SingletonFeature2>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<RealObservableSwitch>))).Returns(new FeatureImpl<RealObservableSwitch>(featureFactory));
            serviceProviderMock.Setup(x => x.GetService(typeof(Feature<RealObservableSwitch2>))).Returns(new FeatureImpl<RealObservableSwitch2>(featureFactory));

            var realObservableSwitch = new FeatureImpl<RealObservableSwitch>(featureFactory);
            var realObservableSubject = new FeatureSubject<RealObservableSwitch>(realObservableSwitch, serviceProvider, service);
            serviceProviderMock.Setup(x => x.GetService(typeof(ObservableFeature<RealObservableSwitch>))).Returns(new ObservableFeatureImpl<RealObservableSwitch>(realObservableSubject));

            var realObservableSwitch2 = new FeatureImpl<RealObservableSwitch2>(featureFactory);
            var realObservableSubject2 = new FeatureSubject<RealObservableSwitch2>(realObservableSwitch2, serviceProvider, service);
            serviceProviderMock.Setup(x => x.GetService(typeof(ObservableFeature<RealObservableSwitch2>))).Returns(new ObservableFeatureImpl<RealObservableSwitch2>(realObservableSubject2));

            bool result = false;

            result = service.ValidateRequiredFeatures(typeof(TransientFeature3));
            Assert.False(result);

            realObservableSwitch2.Value.IsEnabled = false;
            realObservableSubject2.OnNext(realObservableSwitch2.Value);

            result = service.ValidateRequiredFeatures(typeof(TransientFeature3));
            Assert.False(result);

            scopedFeature.IsEnabled = false;

            result = service.ValidateRequiredFeatures(typeof(TransientFeature3));
            Assert.False(result);

            transientFeature.IsEnabled = false;

            result = service.ValidateRequiredFeatures(typeof(TransientFeature3));
            Assert.True(result);
        }

    }
}
