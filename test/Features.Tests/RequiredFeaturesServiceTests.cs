using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Traits;
using Microsoft.Framework.DependencyInjection;
using Moq;
using System;
using Xunit;

namespace Features.Tests
{
    public class RequiredFeaturesServiceTests
    {
        class TransientFeature : Trait
        {
            public TransientFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        class ScopedFeature : Trait
        {
            public ScopedFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        class SingletonFeature : Trait
        {
            public SingletonFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        class RealObservableTrait : ObservableTrait
        {
            public RealObservableTrait(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(RealObservableTrait))]
        [RequiredFeature(typeof(RealObservableTrait2), true)]
        [RequiredFeature(typeof(SingletonFeature))]
        [RequiredFeature(typeof(ScopedFeature), true)]
        [RequiredFeature(typeof(TransientFeature), true)]
        class TransientFeature2 : Trait
        {
            public TransientFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(RealObservableTrait), true)]
        [RequiredFeature(typeof(SingletonFeature2))]
        [RequiredFeature(typeof(ScopedFeature), true)]
        class ScopedFeature2 : Trait
        {
            public ScopedFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(RealObservableTrait))]
        [RequiredFeature(typeof(RealObservableTrait2), true)]
        class SingletonFeature2 : Trait
        {
            public SingletonFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(SingletonFeature), true)]
        class RealObservableTrait2 : ObservableTrait
        {
            public RealObservableTrait2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        class TransientObservableTrait : ObservableTrait
        {
            public TransientObservableTrait(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        class ScopedObservableTrait : ObservableTrait
        {
            public ScopedObservableTrait(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
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
            servicesCollection.AddSingleton<RealObservableTrait>();
            servicesCollection.AddSingleton<RealObservableTrait2>();

            var featureDescriberProvider = new FeatureDescriberProvider(
                new FeatureServicesCollection(servicesCollection), new FeatureDescriberFactory());

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceProvider = serviceProviderMock.Object;

            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            serviceProviderMock.Setup(x => x.GetService(typeof(TransientFeature))).Returns(() => new TransientFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(TransientFeature2))).Returns(() => new TransientFeature2(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(ScopedFeature))).Returns(new ScopedFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(ScopedFeature2))).Returns(new ScopedFeature2(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(SingletonFeature))).Returns(new SingletonFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(SingletonFeature2))).Returns(new SingletonFeature2(service));
            var realObservableTrait = new RealObservableTrait(service);
            var realObservableSubject = new FeatureSubject<RealObservableTrait>(realObservableTrait);
            serviceProviderMock.Setup(x => x.GetService(typeof(RealObservableTrait))).Returns(realObservableTrait);
            serviceProviderMock.Setup(x => x.GetService(typeof(IObservableFeature<RealObservableTrait>))).Returns(new ObservableFeature<RealObservableTrait>(realObservableSubject));
            var realObservableTrait2 = new RealObservableTrait2(service);
            var realObservableSubject2 = new FeatureSubject<RealObservableTrait2>(realObservableTrait2);
            serviceProviderMock.Setup(x => x.GetService(typeof(RealObservableTrait2))).Returns(realObservableTrait2);
            serviceProviderMock.Setup(x => x.GetService(typeof(IObservableFeature<RealObservableTrait2>))).Returns(new ObservableFeature<RealObservableTrait2>(realObservableSubject2));

            IValidateFeatureService result = null;
            result = service.ValidateFeaturesAreInTheCorrectState(typeof(TransientFeature));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(TransientFeature2));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(ScopedFeature));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(ScopedFeature2));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(SingletonFeature));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(SingletonFeature2));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(RealObservableTrait));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(RealObservableTrait2));
            Assert.True(result.Validate());

            Assert.Equal(8, featureDescriberProvider.Describers.Count);

            realObservableTrait.IsEnabled = false;
            //realObservableSubject.OnNext(realObservableTrait);

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
            servicesCollection.AddSingleton<RealObservableTrait>();
            servicesCollection.AddSingleton<RealObservableTrait2>();

            var featureDescriberProvider = new FeatureDescriberProvider(
                new FeatureServicesCollection(servicesCollection), new FeatureDescriberFactory());

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceProvider = serviceProviderMock.Object;

            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            serviceProviderMock.Setup(x => x.GetService(typeof(TransientFeature))).Returns(() => new TransientFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(TransientFeature2))).Returns(() => new TransientFeature2(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(ScopedFeature))).Returns(new ScopedFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(ScopedFeature2))).Returns(new ScopedFeature2(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(SingletonFeature))).Returns(new SingletonFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(SingletonFeature2))).Returns(new SingletonFeature2(service));
            var realObservableTrait = new RealObservableTrait(service);
            var realObservableSubject = new FeatureSubject<RealObservableTrait>(realObservableTrait);
            serviceProviderMock.Setup(x => x.GetService(typeof(RealObservableTrait))).Returns(realObservableTrait);
            serviceProviderMock.Setup(x => x.GetService(typeof(IObservableFeature<RealObservableTrait>))).Returns(new ObservableFeature<RealObservableTrait>(realObservableSubject));
            var realObservableTrait2 = new RealObservableTrait2(service);
            var realObservableSubject2 = new FeatureSubject<RealObservableTrait2>(realObservableTrait2);
            serviceProviderMock.Setup(x => x.GetService(typeof(RealObservableTrait2))).Returns(realObservableTrait2);
            serviceProviderMock.Setup(x => x.GetService(typeof(IObservableFeature<RealObservableTrait2>))).Returns(new ObservableFeature<RealObservableTrait2>(realObservableSubject2));

            IValidateFeatureService result = null;

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(TransientFeature2));
            Assert.True(result.Validate());

            realObservableTrait.IsEnabled = false;
            Assert.True(result.Validate());
            realObservableSubject.OnNext(realObservableTrait);
            Assert.False(result.Validate());

            realObservableTrait.IsEnabled = true;
            Assert.False(result.Validate());
            realObservableSubject.OnNext(realObservableTrait);
            Assert.True(result.Validate());

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
            servicesCollection.AddSingleton<RealObservableTrait>();
            servicesCollection.AddSingleton<RealObservableTrait2>();

            var featureDescriberProvider = new FeatureDescriberProvider(
                new FeatureServicesCollection(servicesCollection), new FeatureDescriberFactory());

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceProvider = serviceProviderMock.Object;

            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            var transientFeature = new TransientFeature(service);
            transientFeature.IsEnabled = false;

            serviceProviderMock.Setup(x => x.GetService(typeof(TransientFeature))).Returns(() => transientFeature);
            serviceProviderMock.Setup(x => x.GetService(typeof(TransientFeature2))).Returns(() => new TransientFeature2(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(ScopedFeature))).Returns(new ScopedFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(ScopedFeature2))).Returns(new ScopedFeature2(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(SingletonFeature))).Returns(new SingletonFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(SingletonFeature2))).Returns(new SingletonFeature2(service));
            var realObservableTrait = new RealObservableTrait(service);
            var realObservableSubject = new FeatureSubject<RealObservableTrait>(realObservableTrait);
            serviceProviderMock.Setup(x => x.GetService(typeof(RealObservableTrait))).Returns(realObservableTrait);
            serviceProviderMock.Setup(x => x.GetService(typeof(IObservableFeature<RealObservableTrait>))).Returns(new ObservableFeature<RealObservableTrait>(realObservableSubject));
            var realObservableTrait2 = new RealObservableTrait2(service);
            var realObservableSubject2 = new FeatureSubject<RealObservableTrait2>(realObservableTrait2);
            serviceProviderMock.Setup(x => x.GetService(typeof(RealObservableTrait2))).Returns(realObservableTrait2);
            serviceProviderMock.Setup(x => x.GetService(typeof(IObservableFeature<RealObservableTrait2>))).Returns(new ObservableFeature<RealObservableTrait2>(realObservableSubject2));

            IValidateFeatureService result = null;
            result = service.ValidateFeaturesAreInTheCorrectState(typeof(TransientFeature));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(TransientFeature2));
            Assert.False(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(ScopedFeature));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(ScopedFeature2));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(SingletonFeature));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(SingletonFeature2));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(RealObservableTrait));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(RealObservableTrait2));
            Assert.True(result.Validate());

            Assert.Equal(8, featureDescriberProvider.Describers.Count);

            realObservableTrait.IsEnabled = false;
            //realObservableSubject.OnNext(realObservableTrait);

        }

        [RequiredFeature(typeof(RealObservableTrait))]
        [RequiredFeature(typeof(RealObservableTrait2), false)]
        [RequiredFeature(typeof(SingletonFeature), true)]
        [RequiredFeature(typeof(ScopedFeature), false)]
        [RequiredFeature(typeof(TransientFeature), false)]
        class TransientFeature3 : Trait
        {
            public TransientFeature3(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
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
            servicesCollection.AddSingleton<RealObservableTrait>();
            servicesCollection.AddSingleton<RealObservableTrait2>();

            var featureDescriberProvider = new FeatureDescriberProvider(
                new FeatureServicesCollection(servicesCollection), new FeatureDescriberFactory());

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceProvider = serviceProviderMock.Object;

            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            var transientFeature = new TransientFeature(service);

            serviceProviderMock.Setup(x => x.GetService(typeof(TransientFeature))).Returns(() => transientFeature);
            var transientFeature3 = new TransientFeature3(service);
            serviceProviderMock.Setup(x => x.GetService(typeof(TransientFeature3))).Returns(() => transientFeature3);
            var scopedFeature = new ScopedFeature(service);
            serviceProviderMock.Setup(x => x.GetService(typeof(ScopedFeature))).Returns(scopedFeature);
            serviceProviderMock.Setup(x => x.GetService(typeof(ScopedFeature2))).Returns(new ScopedFeature2(service));
            var singletonFeature = new SingletonFeature(service);
            serviceProviderMock.Setup(x => x.GetService(typeof(SingletonFeature))).Returns(singletonFeature);
            serviceProviderMock.Setup(x => x.GetService(typeof(SingletonFeature2))).Returns(new SingletonFeature2(service));
            var realObservableTrait = new RealObservableTrait(service);
            var realObservableSubject = new FeatureSubject<RealObservableTrait>(realObservableTrait);
            serviceProviderMock.Setup(x => x.GetService(typeof(RealObservableTrait))).Returns(realObservableTrait);
            serviceProviderMock.Setup(x => x.GetService(typeof(IObservableFeature<RealObservableTrait>))).Returns(new ObservableFeature<RealObservableTrait>(realObservableSubject));
            var realObservableTrait2 = new RealObservableTrait2(service);
            var realObservableSubject2 = new FeatureSubject<RealObservableTrait2>(realObservableTrait2);
            serviceProviderMock.Setup(x => x.GetService(typeof(RealObservableTrait2))).Returns(realObservableTrait2);
            serviceProviderMock.Setup(x => x.GetService(typeof(IObservableFeature<RealObservableTrait2>))).Returns(new ObservableFeature<RealObservableTrait2>(realObservableSubject2));

            IValidateFeatureService result = null;

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(TransientFeature3));
            Assert.False(result.Validate());

            realObservableTrait2.IsEnabled = false;
            realObservableSubject2.OnNext(realObservableTrait2);

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(TransientFeature3));
            Assert.False(result.Validate());

            scopedFeature.IsEnabled = false;

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(TransientFeature3));
            Assert.False(result.Validate());

            transientFeature.IsEnabled = false;

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(TransientFeature3));
            Assert.True(result.Validate());
        }

    }
}
