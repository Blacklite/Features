using System;
using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using NSubstitute;
using Xunit;

namespace Features.Tests.Factory
{
    public class FeatureFactoryTests
    {
        public class Feature2 : IFeature { }

        [Fact]
        public void GetFeatureCachesInstances()
        {
            var featureCompositionProvider = Substitute.For<IFeatureCompositionProvider>();
            var featureDescriberProvider = Substitute.For<IFeatureDescriberProvider>();

            var factory = new FeatureFactory(featureCompositionProvider, featureDescriberProvider);

            var result = factory.GetFeature(typeof(Feature2));

            var result2 = factory.GetFeature(typeof(Feature2));

            Assert.Same(result, result2);
        }

        class MockComposer : IFeatureComposition
        {
            private readonly object _feature;
            public MockComposer(object feature)
            {
                this._feature = feature;
            }

            public int Priority
            {
                get { throw new NotImplementedException(); }
            }

            public T Configure<T>(T feature, IFeatureDescriber describer)
            {
                if (this._feature != null)
                    return (T)this._feature;
                return feature;
            }

            public bool IsApplicableTo(IFeatureDescriber describer)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void GetFeatureAggregatesEvenWithNewInstances()
        {
            var featureCompositionProvider = Substitute.For<IFeatureCompositionProvider>();
            var featureDescriberProvider = Substitute.For<IFeatureDescriberProvider>();

            var composer = new MockComposer(null);

            var newInstance = new Feature2();
            var composerNewInstance = new MockComposer(newInstance);

            featureCompositionProvider.GetComposers<Feature2>()
                .Returns(new[] { composerNewInstance, composer });

            var factory = new FeatureFactory(featureCompositionProvider, featureDescriberProvider);

            var result = factory.GetFeature(typeof(Feature2));

            Assert.Same(newInstance, result);
        }
    }
}
