using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Blacklite.Framework.Features.OptionsModel;
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
            var optionsComposer = Substitute.For<IEnumerable<IPreFeatureComposition>>();
            var requiredComposer = Substitute.For<IEnumerable<IPostFeatureComposition>>();
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();

            var provider = new FeatureCompositionProvider(
                composers,
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

            var optionsComposer = Substitute.For<IEnumerable<IPreFeatureComposition>>();
            var requiredComposer = Substitute.For<IEnumerable<IPostFeatureComposition>>();
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

            var optionsComposer = new OptionsFeatureComposer(Substitute.For<IFeatureFactory>(), Substitute.For<IFeatureOptionsProvider>());
            var requiredComposer = new RequiredFeatureComposer(Substitute.For<IRequiredFeaturesService>());
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();

            var describer = Substitute.For<IFeatureDescriber>();
            describer.HasOptions.Returns(true);
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
                new[] { optionsComposer },
                new[] { requiredComposer },
                describerProvider
            );

            var result = provider.GetComposers<Feature2>();
            Assert.Same(optionsComposer, result.OfType<IEnumerable<IPreFeatureComposition>>().Single());
        }

        [Fact]
        public void GetsRequireComposer()
        {
            var composers = Enumerable.Empty<IFeatureComposition>();

            var optionsComposer = new OptionsFeatureComposer(Substitute.For<IFeatureFactory>(), Substitute.For<IFeatureOptionsProvider>());
            var requiredComposer = new RequiredFeatureComposer(Substitute.For<IRequiredFeaturesService>());
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();

            var describer = Substitute.For<IFeatureDescriber>();
            describer.Requires.Returns(new RequiredFeatureAttribute[1]);
            describer.HasEnabled.Returns(true);
            describer.IsReadOnly.Returns(false);
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
                new[] { optionsComposer },
                new[] { requiredComposer },
                describerProvider
            );

            var result = provider.GetComposers<Feature2>();
            Assert.Same(requiredComposer, result.OfType<IEnumerable<IPostFeatureComposition>>().Single());
        }

        [Fact]
        public void CachesResults()
        {
            var composers = Enumerable.Empty<IFeatureComposition>();

            var optionsComposer = new OptionsFeatureComposer(Substitute.For<IFeatureFactory>(), Substitute.For<IFeatureOptionsProvider>());
            var requiredComposer = new RequiredFeatureComposer(Substitute.For<IRequiredFeaturesService>());
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();

            var describer = Substitute.For<IFeatureDescriber>();
            describer.HasOptions.Returns(true);
            describer.Requires.Returns(new RequiredFeatureAttribute[1]);
            describer.HasEnabled.Returns(true);
            describer.IsReadOnly.Returns(false);
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
                new[] { optionsComposer },
                new[] { requiredComposer },
                describerProvider
            );

            var result = provider.GetComposers<Feature2>();
            var result2 = provider.GetComposers<Feature2>();
            Assert.Same(result, result2);
        }
    }
}
