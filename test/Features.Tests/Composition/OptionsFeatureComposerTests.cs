using Microsoft.Framework.DependencyInjection;
using Blacklite.Framework.Features.OptionsModel;
using System;
using System.Linq;
using System.Reflection;
using Blacklite.Framework.Features.Describers;
using Xunit;
using Blacklite.Framework.Features;
using NSubstitute;
using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Factory;

namespace Features.Tests.Composition
{
    public class OptionsFeatureComposerTests
    {
        public class Options { public string Property { get; set; } }
        class FeatureOptions : Feature { public string Property { get; set; } }
        class SwitchOptions : Switch<Options> { }
        class SwitchFeatureOptions : Switch<FeatureOptions> { }

        class SwitchNoOptions : Switch { }

        [Fact]
        public void IsOnlyApplicableToTypesWithTheProperAttribute()
        {
            var composer = new OptionsFeatureComposer(Substitute.For<IFeatureFactory>(), Substitute.For<IFeatureOptionsProvider>());

            var describer = Substitute.For<IFeatureDescriber>();
            describer.HasOptions.Returns(true);
            Assert.True(composer.IsApplicableTo(describer));

            describer = Substitute.For<IFeatureDescriber>();
            describer.HasOptions.Returns(false);
            Assert.False(composer.IsApplicableTo(describer));
        }

        [Fact]
        public void SetFeatureBasedOptions()
        {
            var featureFactory = Substitute.For<IFeatureFactory>();
            featureFactory.GetFeature(typeof(FeatureOptions)).Returns(new FeatureOptions());
            var featureSub = new FeatureImpl<FeatureOptions>(featureFactory);
            featureFactory.GetFeature(typeof(Feature<FeatureOptions>)).Returns(featureSub.Value);

            var composer = new OptionsFeatureComposer(featureFactory, Substitute.For<IFeatureOptionsProvider>());
            var feature = new SwitchFeatureOptions();

            var describer = new FeatureDescriberFactory().Create(new[] { typeof(SwitchFeatureOptions).GetTypeInfo() }).Single();
            composer.Configure(feature, describer);

            Assert.Same(featureSub.Value, feature.Options);
        }

        [Fact]
        public void SetOptions()
        {
            var optionsProvider = Substitute.For<IFeatureOptionsProvider>();
            var sub = Substitute.For<IFeatureOptions<Options>>();
            optionsProvider.GetOptions(typeof(Options)).Returns(sub.Options);

            var composer = new OptionsFeatureComposer(Substitute.For<IFeatureFactory>(), optionsProvider);
            var feature = new SwitchOptions();

            var describer = new FeatureDescriberFactory().Create(new[] { typeof(SwitchOptions).GetTypeInfo() }).Single();
            composer.Configure(feature, describer);

            Assert.Same(sub.Options, feature.Options);
        }
    }
}
