using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Resolvers;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Xunit;

namespace Features.Tests
{
    public class FeatureProviderTests
    {
        private class Scoped : ScopedFeature { }
        private class Simple : Feature { }
        public abstract class FeatureResolver : IFeatureResolver
        {
            public abstract int Priority { get; }

            public abstract bool CanResolve<T>(IFeatureResolutionContext context) where T : IFeature;
            public abstract Type GetFeatureType();
            public abstract T Resolve<T>(IFeatureResolutionContext context) where T : IFeature;
        }

        [Fact]
        public void FeatureProviderReturnsAFeature()
        {
            var serviceProvider = Mock.Of<IServiceProvider>();
            var scoped = new Scoped();
            var simple = new Simple();

            var scopedResolver = new Mock<FeatureResolver>();
            scopedResolver.Setup(x => x.CanResolve<Scoped>(It.IsAny<IFeatureResolutionContext>())).Returns(true);
            scopedResolver.Setup(x => x.Resolve<Scoped>(It.IsAny<IFeatureResolutionContext>())).Returns(new Scoped());

            var featureResolverProviderMock = new Mock<IFeatureResolverProvider>();
            featureResolverProviderMock.Setup(x => x.Resolvers).Returns(new ReadOnlyDictionary<Type, IEnumerable<IFeatureResolverDescriptor>>(
                new Dictionary<Type, IEnumerable<IFeatureResolverDescriptor>>()
                {
                    [typeof(Scoped)] = new[] { new FeatureResolverDescriptor(scopedResolver.Object) }
                }));
            var featureResolverProvider = featureResolverProviderMock.Object;

            var globalFeatureProviderMock = new Mock<IGlobalFeatureProvider>();
            globalFeatureProviderMock.SetupGet(x => x.FeatureDescribers).Returns(new Dictionary<Type, IFeatureDescriber>()
            {
                [typeof(Scoped)] = FeatureDescriber.Create(typeof(Scoped).GetTypeInfo()),
                [typeof(Simple)] = FeatureDescriber.Create(typeof(Simple).GetTypeInfo()),
            });

            globalFeatureProviderMock.Setup(x => x.GetFeature<Simple>()).Returns(simple);

            var globalFeatureProvider = globalFeatureProviderMock.Object;

            var provider = new FeatureProvider(serviceProvider, featureResolverProvider, globalFeatureProvider);

            var scopedResult = provider.GetFeature<Scoped>();
            Assert.NotEqual(scoped, scopedResult);
            Assert.IsType<Scoped>(scopedResult);
            var scopedResult2 = provider.GetFeature<Scoped>();
            Assert.Equal(scopedResult, scopedResult2);

            var simpleResult = provider.GetFeature<Simple>();
            Assert.Equal(simple, simpleResult);
            var simpleResult2 = provider.GetFeature<Simple>();
            Assert.Equal(simpleResult, simpleResult2);
        }

        [Fact]
        public void GlobalFeatureProviderReturnsAFeature()
        {
            var serviceProvider = Mock.Of<IServiceProvider>();
            var scoped = new Scoped();
            var simple = new Simple();

            var scopedResolver = new Mock<FeatureResolver>();
            scopedResolver.Setup(x => x.CanResolve<Scoped>(It.IsAny<IFeatureResolutionContext>())).Returns(true);
            scopedResolver.Setup(x => x.Resolve<Scoped>(It.IsAny<IFeatureResolutionContext>())).Returns(scoped);

            var simpleResolver = new Mock<FeatureResolver>();
            simpleResolver.Setup(x => x.CanResolve<Simple>(It.IsAny<IFeatureResolutionContext>())).Returns(true);
            simpleResolver.Setup(x => x.Resolve<Simple>(It.IsAny<IFeatureResolutionContext>())).Returns(simple);

            var featureResolverProviderMock = new Mock<IFeatureResolverProvider>();
            featureResolverProviderMock.Setup(x => x.Resolvers).Returns(new ReadOnlyDictionary<Type, IEnumerable<IFeatureResolverDescriptor>>(
                new Dictionary<Type, IEnumerable<IFeatureResolverDescriptor>>()
                {
                    [typeof(Scoped)] = new[] { new FeatureResolverDescriptor(scopedResolver.Object) },
                    [typeof(Simple)] = new[] { new FeatureResolverDescriptor(simpleResolver.Object) }
                }));
            var featureResolverProvider = featureResolverProviderMock.Object;

            var featureDescriberProviderMock = new Mock<IFeatureDescriberProvider>();
            featureDescriberProviderMock.Setup(x => x.Describers).Returns(new[] { FeatureDescriber.Create(typeof(Scoped).GetTypeInfo()), FeatureDescriber.Create(typeof(Simple).GetTypeInfo()) });

            var featureDescriberProvider = featureDescriberProviderMock.Object;

            var provider = new GlobalFeatureProvider(serviceProvider, featureDescriberProvider, featureResolverProvider);

            var scopedResult = provider.GetFeature<Scoped>();
            Assert.Equal(scoped, scopedResult);

            var simpleResult = provider.GetFeature<Simple>();
            Assert.Equal(simple, simpleResult);
        }
    }
}
