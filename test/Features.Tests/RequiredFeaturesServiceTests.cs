using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Blacklite.Framework.Features.Observables;
using Microsoft.Framework.DependencyInjection;
using NSubstitute;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Features.Tests
{
    public class RequiredFeaturesServiceTests
    {
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
        class ScopedFeature2 : Switch
        {
        }

        [RequiredFeature(typeof(RealObservableSwitch))]
        [RequiredFeature(typeof(RealObservableSwitch2), true)]
        class SingletonFeature2 : Switch
        {
        }

        class RealObservableSwitch2 : ObservableSwitch
        {
        }

        class TransientObservableSwitch : ObservableSwitch
        {
        }

        [Fact]
        public void ReturnsTrueByDefault()
        {

            var servicesCollection = new[] {
            typeof(ScopedFeature),
            typeof(ScopedFeature2),
            typeof(SingletonFeature),
            typeof(SingletonFeature2),
            typeof(RealObservableSwitch),
            typeof(RealObservableSwitch2),
            }.Select(x => x.GetTypeInfo());

            Substitute.For<IFeatureTypeProvider>();

            var featureTypeProvider = Substitute.For<IFeatureTypeProvider>();
            featureTypeProvider.FeatureTypes.Returns(x => servicesCollection);

            var featureDescriberProvider = new FeatureDescriberProvider(featureTypeProvider, new FeatureDescriberFactory());


            var featureFactory = Substitute.For<IFeatureFactory>();
            featureFactory.GetFeature(typeof(ScopedFeature)).Returns(x => new ScopedFeature());
            featureFactory.GetFeature(typeof(ScopedFeature2)).Returns(x => new ScopedFeature2());
            featureFactory.GetFeature(typeof(SingletonFeature)).Returns(x => new SingletonFeature());
            featureFactory.GetFeature(typeof(SingletonFeature2)).Returns(x => new SingletonFeature2());
            featureFactory.GetFeature(typeof(RealObservableSwitch)).Returns(x => new RealObservableSwitch());
            featureFactory.GetFeature(typeof(RealObservableSwitch2)).Returns(x => new RealObservableSwitch2());


            var featureSubjectFactory = Substitute.For<IFeatureSubjectFactory>();
            var observableFeatureFactory = new ObservableFeatureFactory(featureSubjectFactory);

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Any<Type>()).Returns(x =>
            {
                var arg = (Type)x.Args()[0];
                if (arg.GetGenericTypeDefinition() == typeof(Feature<>))
                    return Activator.CreateInstance(typeof(FeatureImpl<>).MakeGenericType(arg.GetGenericArguments()[0]), featureFactory);

                return Activator.CreateInstance(typeof(ObservableFeatureImpl<>).MakeGenericType(arg.GetGenericArguments()[0]), featureSubjectFactory);
            });
            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            var realObservableSwitch = new FeatureImpl<RealObservableSwitch>(featureFactory);
            var realObservableSubject = new FeatureSubject<RealObservableSwitch>(realObservableSwitch, featureFactory, service, featureDescriberProvider, featureSubjectFactory);
            featureSubjectFactory.GetSubject(typeof(RealObservableSwitch)).Returns(x => realObservableSubject);

            var realObservableSwitch2 = new FeatureImpl<RealObservableSwitch2>(featureFactory);
            var realObservableSubject2 = new FeatureSubject<RealObservableSwitch2>(realObservableSwitch2, featureFactory, service, featureDescriberProvider, featureSubjectFactory);
            featureSubjectFactory.GetSubject(typeof(RealObservableSwitch2)).Returns(x => realObservableSubject2);

            bool result = false;
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

            Assert.Equal(6, featureDescriberProvider.Describers.Count);
        }

        [Fact]
        public void HandlesObservables()
        {

            var servicesCollection = new[] {
            typeof(ScopedFeature),
            typeof(ScopedFeature2),
            typeof(SingletonFeature),
            typeof(SingletonFeature2),
            typeof(RealObservableSwitch),
            typeof(RealObservableSwitch2),
            }.Select(x => x.GetTypeInfo());

            var featureTypeProvider = Substitute.For<IFeatureTypeProvider>();
            featureTypeProvider.FeatureTypes.Returns(x => servicesCollection);


            var featureDescriberProvider = new FeatureDescriberProvider(featureTypeProvider, new FeatureDescriberFactory());

            var featureFactory = Substitute.For<IFeatureFactory>();
            featureFactory.GetFeature(typeof(ScopedFeature)).Returns(x => new ScopedFeature());
            featureFactory.GetFeature(typeof(ScopedFeature2)).Returns(x => new ScopedFeature2());
            featureFactory.GetFeature(typeof(SingletonFeature)).Returns(x => new SingletonFeature());
            featureFactory.GetFeature(typeof(SingletonFeature2)).Returns(x => new SingletonFeature2());
            featureFactory.GetFeature(typeof(RealObservableSwitch)).Returns(x => new RealObservableSwitch());
            featureFactory.GetFeature(typeof(RealObservableSwitch2)).Returns(x => new RealObservableSwitch2());

            var featureSubjectFactory = Substitute.For<IFeatureSubjectFactory>();
            var observableFeatureFactory = new ObservableFeatureFactory(featureSubjectFactory);

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Any<Type>()).Returns(x =>
            {
                var arg = (Type)x.Args()[0];
                if (arg.GetGenericTypeDefinition() == typeof(Feature<>))
                    return Activator.CreateInstance(typeof(FeatureImpl<>).MakeGenericType(arg.GetGenericArguments()[0]), featureFactory);

                return Activator.CreateInstance(typeof(ObservableFeatureImpl<>).MakeGenericType(arg.GetGenericArguments()[0]), featureSubjectFactory);
            });
            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            var realObservableSwitch = new FeatureImpl<RealObservableSwitch>(featureFactory);
            var realObservableSubject = new FeatureSubject<RealObservableSwitch>(realObservableSwitch, featureFactory, service, featureDescriberProvider, featureSubjectFactory);
            featureSubjectFactory.GetSubject(typeof(RealObservableSwitch)).Returns(x => realObservableSubject);

            var realObservableSwitch2 = new FeatureImpl<RealObservableSwitch2>(featureFactory);
            var realObservableSubject2 = new FeatureSubject<RealObservableSwitch2>(realObservableSwitch2, featureFactory, service, featureDescriberProvider, featureSubjectFactory);
            featureSubjectFactory.GetSubject(typeof(RealObservableSwitch2)).Returns(x => realObservableSubject2);

            bool result = false;

            result = service.ValidateRequiredFeatures(typeof(ScopedFeature2));
            Assert.True(result); 

            realObservableSwitch.Value.IsEnabled = false;
            realObservableSubject.OnNext(realObservableSwitch.Value);

            result = service.ValidateRequiredFeatures(typeof(ScopedFeature2));
            Assert.False(result);

            realObservableSwitch.Value.IsEnabled = true;
            realObservableSubject.OnNext(realObservableSwitch.Value);

            result = service.ValidateRequiredFeatures(typeof(ScopedFeature2));
            Assert.True(result);

        }

        [Fact]
        public void ReturnsFalseWhenAppropriate()
        {

            var servicesCollection = new[] {
            typeof(ScopedFeature),
            typeof(ScopedFeature2),
            typeof(SingletonFeature),
            typeof(SingletonFeature2),
            typeof(RealObservableSwitch),
            typeof(RealObservableSwitch2),
            }.Select(x => x.GetTypeInfo());

            var featureTypeProvider = Substitute.For<IFeatureTypeProvider>();
            featureTypeProvider.FeatureTypes.Returns(x => servicesCollection);


            var featureDescriberProvider = new FeatureDescriberProvider(featureTypeProvider, new FeatureDescriberFactory());


            var scopedFeature = new ScopedFeature();
            scopedFeature.IsEnabled = false;

            var featureFactory = Substitute.For<IFeatureFactory>();
            featureFactory.GetFeature(typeof(ScopedFeature)).Returns(x => scopedFeature);
            featureFactory.GetFeature(typeof(ScopedFeature2)).Returns(x => new ScopedFeature2());
            featureFactory.GetFeature(typeof(SingletonFeature)).Returns(x => new SingletonFeature());
            featureFactory.GetFeature(typeof(SingletonFeature2)).Returns(x => new SingletonFeature2());
            featureFactory.GetFeature(typeof(RealObservableSwitch)).Returns(x => new RealObservableSwitch());
            featureFactory.GetFeature(typeof(RealObservableSwitch2)).Returns(x => new RealObservableSwitch2());


            var featureSubjectFactory = Substitute.For<IFeatureSubjectFactory>();
            var observableFeatureFactory = new ObservableFeatureFactory(featureSubjectFactory);

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Any<Type>()).Returns(x =>
            {
                var arg = (Type)x.Args()[0];
                if (arg.GetGenericTypeDefinition() == typeof(Feature<>))
                    return Activator.CreateInstance(typeof(FeatureImpl<>).MakeGenericType(arg.GetGenericArguments()[0]), featureFactory);

                return Activator.CreateInstance(typeof(ObservableFeatureImpl<>).MakeGenericType(arg.GetGenericArguments()[0]), featureSubjectFactory);
            });
            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            var realObservableSwitch = new FeatureImpl<RealObservableSwitch>(featureFactory);
            var realObservableSubject = new FeatureSubject<RealObservableSwitch>(realObservableSwitch, featureFactory, service, featureDescriberProvider, featureSubjectFactory);
            featureSubjectFactory.GetSubject(typeof(RealObservableSwitch)).Returns(x => realObservableSubject);

            var realObservableSwitch2 = new FeatureImpl<RealObservableSwitch2>(featureFactory);
            var realObservableSubject2 = new FeatureSubject<RealObservableSwitch2>(realObservableSwitch2, featureFactory, service, featureDescriberProvider, featureSubjectFactory);
            featureSubjectFactory.GetSubject(typeof(RealObservableSwitch2)).Returns(x => realObservableSubject2);

            bool result = false;
            result = service.ValidateRequiredFeatures(typeof(ScopedFeature));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(ScopedFeature2));
            Assert.False(result);

            result = service.ValidateRequiredFeatures(typeof(SingletonFeature));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(SingletonFeature2));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(RealObservableSwitch));
            Assert.True(result);

            result = service.ValidateRequiredFeatures(typeof(RealObservableSwitch2));
            Assert.True(result);

            Assert.Equal(6, featureDescriberProvider.Describers.Count);
        }

        [RequiredFeature(typeof(RealObservableSwitch))]
        [RequiredFeature(typeof(RealObservableSwitch2), false)]
        [RequiredFeature(typeof(SingletonFeature), true)]
        [RequiredFeature(typeof(ScopedFeature), false)]
        class ScopedFeature3 : Switch
        {
        }

        [Fact]
        public void ReturnsFalseWhenAppropriate2()
        {
            var servicesCollection = new[] {
            typeof(ScopedFeature),
            typeof(ScopedFeature3),
            typeof(SingletonFeature),
            typeof(SingletonFeature2),
            typeof(RealObservableSwitch),
            typeof(RealObservableSwitch2),
            }.Select(x => x.GetTypeInfo());

            var featureTypeProvider = Substitute.For<IFeatureTypeProvider>();
            featureTypeProvider.FeatureTypes.Returns(x => servicesCollection);


            var featureDescriberProvider = new FeatureDescriberProvider(featureTypeProvider, new FeatureDescriberFactory());

            var ScopedFeature = new ScopedFeature();
            var singletonFeature = new SingletonFeature();
            var featureFactory = Substitute.For<IFeatureFactory>();
            featureFactory.GetFeature(typeof(ScopedFeature)).Returns(x => ScopedFeature);
            featureFactory.GetFeature(typeof(ScopedFeature3)).Returns(x => new ScopedFeature3());
            featureFactory.GetFeature(typeof(SingletonFeature)).Returns(x => singletonFeature);
            featureFactory.GetFeature(typeof(SingletonFeature2)).Returns(x => new SingletonFeature2());
            featureFactory.GetFeature(typeof(RealObservableSwitch)).Returns(x => new RealObservableSwitch());
            featureFactory.GetFeature(typeof(RealObservableSwitch2)).Returns(x => new RealObservableSwitch2());


            var featureSubjectFactory = Substitute.For<IFeatureSubjectFactory>();
            var observableFeatureFactory = new ObservableFeatureFactory(featureSubjectFactory);

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Any<Type>()).Returns(x =>
            {
                var arg = (Type)x.Args()[0];
                if (arg.GetGenericTypeDefinition() == typeof(Feature<>))
                    return Activator.CreateInstance(typeof(FeatureImpl<>).MakeGenericType(arg.GetGenericArguments()[0]), featureFactory);

                return Activator.CreateInstance(typeof(ObservableFeatureImpl<>).MakeGenericType(arg.GetGenericArguments()[0]), featureSubjectFactory);
            });
            var service = new RequiredFeaturesService(serviceProvider, featureDescriberProvider);

            var realObservableSwitch = new FeatureImpl<RealObservableSwitch>(featureFactory);
            var realObservableSubject = new FeatureSubject<RealObservableSwitch>(realObservableSwitch, featureFactory, service, featureDescriberProvider, featureSubjectFactory);
            featureSubjectFactory.GetSubject(typeof(RealObservableSwitch)).Returns(x => realObservableSubject);

            var realObservableSwitch2 = new FeatureImpl<RealObservableSwitch2>(featureFactory);
            var realObservableSubject2 = new FeatureSubject<RealObservableSwitch2>(realObservableSwitch2, featureFactory, service, featureDescriberProvider, featureSubjectFactory);
            featureSubjectFactory.GetSubject(typeof(RealObservableSwitch2)).Returns(x => realObservableSubject2);

            bool result = false;

            result = service.ValidateRequiredFeatures(typeof(ScopedFeature3));
            Assert.False(result);

            realObservableSwitch2.Value.IsEnabled = false;
            realObservableSubject2.OnNext(realObservableSwitch2.Value);

            result = service.ValidateRequiredFeatures(typeof(ScopedFeature3));
            Assert.False(result);

            result = service.ValidateRequiredFeatures(typeof(ScopedFeature3));
            Assert.False(result);

            ScopedFeature.IsEnabled = false;

            result = service.ValidateRequiredFeatures(typeof(ScopedFeature3));
            Assert.True(result);
        }

    }
}
