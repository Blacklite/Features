using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Features.Tests.Factory
{
    public class FeatureCompositionProviderTests
    {
        class Feature2 : IFeature { }

        [Fact]
        public void ThrowsIfNoDescriberFound()
        {
            var composers = Enumerable.Empty<IFeatureComposition>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            var optionsComposer = Substitute.For<IOptionsFeatureComposer>();
            var requiredComposer = Substitute.For<IRequiredFeatureComposer>();
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();

            var provider = new FeatureCompositionProvider(
                composers,
                serviceProvider,
                optionsComposer,
                requiredComposer,
                describerProvider
            );

            Assert.Throws<KeyNotFoundException>(() => provider.GetComposers<Feature2>());
        }

        [Fact]
        public void GetsComposers()
        {
            var composers = Enumerable.Empty<IFeatureComposition>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IEnumerable<IFeatureComposition<Feature2>>))
                .Returns(Enumerable.Empty<IFeatureComposition<Feature2>>());

            var optionsComposer = Substitute.For<IOptionsFeatureComposer>();
            var requiredComposer = Substitute.For<IRequiredFeatureComposer>();
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();

            var describer = Substitute.For<IFeatureDescriber>();
            describerProvider.Describers[typeof(Feature2)].Returns(describer);

            IFeatureDescriber describer2;
            describerProvider.Describers.TryGetValue(typeof(Feature2), out describer2)
                .Returns(x =>
                {
                    x[1] = describer;
                    return true;
                });

            var provider = new FeatureCompositionProvider(
                composers,
                serviceProvider,
                optionsComposer,
                requiredComposer,
                describerProvider
            );

            provider.GetComposers<Feature2>();
        }

        [Fact]
        public void GetsOptionComposer()
        {
            var composers = Enumerable.Empty<IFeatureComposition>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IEnumerable<IFeatureComposition<Feature2>>))
                .Returns(Enumerable.Empty<IFeatureComposition<Feature2>>());

            var optionsComposer = Substitute.For<IOptionsFeatureComposer>();
            var requiredComposer = Substitute.For<IRequiredFeatureComposer>();
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();

            var describer = Substitute.For<IFeatureDescriber>();
            describerProvider.Describers[typeof(Feature2)].Returns(describer);

            IFeatureDescriber describer2;
            describerProvider.Describers.TryGetValue(typeof(Feature2), out describer2)
                .Returns(x =>
                {
                    x[1] = describer;
                    return true;
                });

            var provider = new FeatureCompositionProvider(
                composers,
                serviceProvider,
                optionsComposer,
                requiredComposer,
                describerProvider
            );

            var result = provider.GetComposers<Feature2>();
            Assert.Same(optionsComposer, result.OfType<IOptionsFeatureComposer>().Single());
        }

        [Fact]
        public void GetsRequireComposer()
        {
            var composers = Enumerable.Empty<IFeatureComposition>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IEnumerable<IFeatureComposition<Feature2>>))
                .Returns(Enumerable.Empty<IFeatureComposition<Feature2>>());

            var optionsComposer = Substitute.For<IOptionsFeatureComposer>();
            var requiredComposer = Substitute.For<IRequiredFeatureComposer>();
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();

            var describer = Substitute.For<IFeatureDescriber>();
            describerProvider.Describers[typeof(Feature2)].Returns(describer);

            IFeatureDescriber describer2;
            describerProvider.Describers.TryGetValue(typeof(Feature2), out describer2)
                .Returns(x =>
                {
                    x[1] = describer;
                    return true;
                });

            var provider = new FeatureCompositionProvider(
                composers,
                serviceProvider,
                optionsComposer,
                requiredComposer,
                describerProvider
            );

            var result = provider.GetComposers<Feature2>();
            Assert.Same(requiredComposer, result.OfType<IRequiredFeatureComposer>().Single());
        }

        [Fact]
        public void CachesResults()
        {
            var composers = Enumerable.Empty<IFeatureComposition>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IEnumerable<IFeatureComposition<Feature2>>))
                .Returns(Enumerable.Empty<IFeatureComposition<Feature2>>());

            var optionsComposer = Substitute.For<IOptionsFeatureComposer>();
            var requiredComposer = Substitute.For<IRequiredFeatureComposer>();
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();

            var describer = Substitute.For<IFeatureDescriber>();
            describerProvider.Describers[typeof(Feature2)].Returns(describer);

            IFeatureDescriber describer2;
            describerProvider.Describers.TryGetValue(typeof(Feature2), out describer2)
                .Returns(x =>
                {
                    x[1] = describer;
                    return true;
                });

            var provider = new FeatureCompositionProvider(
                composers,
                serviceProvider,
                optionsComposer,
                requiredComposer,
                describerProvider
            );

            var result = provider.GetComposers<Feature2>();
            var result2 = provider.GetComposers<Feature2>();
            Assert.Same(result, result2);
        }
    }
}
