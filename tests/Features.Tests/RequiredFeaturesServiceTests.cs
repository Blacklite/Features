﻿using Blacklite.Framework.Features;
using Microsoft.Framework.DependencyInjection;
using Moq;
using System;
using Xunit;

namespace Features.Tests
{
    public class RequiredFeaturesServiceTests
    {
        class TransientFeature : Feature
        {
            public TransientFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        class ScopedFeature : Feature
        {
            public ScopedFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        class SingletonFeature : Feature
        {
            public SingletonFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        class RealObservableFeature : ObservableFeature
        {
            public RealObservableFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(RealObservableFeature), true)]
        [RequiredFeature(typeof(RealObservableFeature2), true)]
        [RequiredFeature(typeof(SingletonFeature), true)]
        [RequiredFeature(typeof(ScopedFeature), true)]
        [RequiredFeature(typeof(TransientFeature), true)]
        class TransientFeature2 : Feature
        {
            public TransientFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(RealObservableFeature), true)]
        [RequiredFeature(typeof(SingletonFeature2), true)]
        [RequiredFeature(typeof(ScopedFeature), true)]
        class ScopedFeature2 : Feature
        {
            public ScopedFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(RealObservableFeature), true)]
        [RequiredFeature(typeof(RealObservableFeature2), true)]
        class SingletonFeature2 : Feature
        {
            public SingletonFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
        }

        [RequiredFeature(typeof(SingletonFeature), true)]
        class RealObservableFeature2 : ObservableFeature
        {
            public RealObservableFeature2(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures) { }
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
            servicesCollection.AddSingleton<RealObservableFeature>();
            servicesCollection.AddSingleton<RealObservableFeature2>();

            var featureDescriberProvider = new FeatureDescriberProvider(
                new FeatureServicesCollection(servicesCollection));

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceProvider = serviceProviderMock.Object;

            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            serviceProviderMock.Setup(x => x.GetService(typeof(TransientFeature))).Returns(() => new TransientFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(TransientFeature2))).Returns(() => new TransientFeature2(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(ScopedFeature))).Returns(new ScopedFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(ScopedFeature2))).Returns(new ScopedFeature2(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(SingletonFeature))).Returns(new SingletonFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(SingletonFeature2))).Returns(new SingletonFeature2(service));
            var realObservableFeature = new RealObservableFeature(service);
            var realObservableSubject = new SubjectFeature<RealObservableFeature>(realObservableFeature);
            serviceProviderMock.Setup(x => x.GetService(typeof(RealObservableFeature))).Returns(realObservableFeature);
            serviceProviderMock.Setup(x => x.GetService(typeof(IObservableFeature<RealObservableFeature>))).Returns(new ObservableFeature<RealObservableFeature>(realObservableSubject));
            var realObservableFeature2 = new RealObservableFeature2(service);
            var realObservableSubject2 = new SubjectFeature<RealObservableFeature2>(realObservableFeature2);
            serviceProviderMock.Setup(x => x.GetService(typeof(RealObservableFeature2))).Returns(realObservableFeature2);
            serviceProviderMock.Setup(x => x.GetService(typeof(IObservableFeature<RealObservableFeature2>))).Returns(new ObservableFeature<RealObservableFeature2>(realObservableSubject2));

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

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(RealObservableFeature));
            Assert.True(result.Validate());

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(RealObservableFeature2));
            Assert.True(result.Validate());

            Assert.Equal(8, featureDescriberProvider.Describers.Count);

            realObservableFeature.IsEnabled = false;
            //realObservableSubject.OnNext(realObservableFeature);

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
            servicesCollection.AddSingleton<RealObservableFeature>();
            servicesCollection.AddSingleton<RealObservableFeature2>();

            var featureDescriberProvider = new FeatureDescriberProvider(
                new FeatureServicesCollection(servicesCollection));

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceProvider = serviceProviderMock.Object;

            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            serviceProviderMock.Setup(x => x.GetService(typeof(TransientFeature))).Returns(() => new TransientFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(TransientFeature2))).Returns(() => new TransientFeature2(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(ScopedFeature))).Returns(new ScopedFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(ScopedFeature2))).Returns(new ScopedFeature2(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(SingletonFeature))).Returns(new SingletonFeature(service));
            serviceProviderMock.Setup(x => x.GetService(typeof(SingletonFeature2))).Returns(new SingletonFeature2(service));
            var realObservableFeature = new RealObservableFeature(service);
            var realObservableSubject = new SubjectFeature<RealObservableFeature>(realObservableFeature);
            serviceProviderMock.Setup(x => x.GetService(typeof(RealObservableFeature))).Returns(realObservableFeature);
            serviceProviderMock.Setup(x => x.GetService(typeof(IObservableFeature<RealObservableFeature>))).Returns(new ObservableFeature<RealObservableFeature>(realObservableSubject));
            var realObservableFeature2 = new RealObservableFeature2(service);
            var realObservableSubject2 = new SubjectFeature<RealObservableFeature2>(realObservableFeature2);
            serviceProviderMock.Setup(x => x.GetService(typeof(RealObservableFeature2))).Returns(realObservableFeature2);
            serviceProviderMock.Setup(x => x.GetService(typeof(IObservableFeature<RealObservableFeature2>))).Returns(new ObservableFeature<RealObservableFeature2>(realObservableSubject2));

            IValidateFeatureService result = null;

            result = service.ValidateFeaturesAreInTheCorrectState(typeof(TransientFeature2));
            Assert.True(result.Validate());

            realObservableFeature.IsEnabled = false;
            Assert.True(result.Validate());
            realObservableSubject.OnNext(realObservableFeature);
            Assert.False(result.Validate());

        }
    }
}
