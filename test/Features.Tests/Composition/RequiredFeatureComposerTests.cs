using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Blacklite.Framework.Features.OptionsModel;
using NSubstitute;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Features.Tests.Composition
{
    public class RequiredFeatureComposerTests
    {

        class Switch1 : Switch { }

        [Fact]
        public void IsOnlyApplicableToTypesWithTheProperAttribute()
        {
            var requiredFeatureService = Substitute.For<IRequiredFeaturesService>();

            var composer = new RequiredFeatureComposer(requiredFeatureService);

            var describer = Substitute.For<IFeatureDescriber>();
            describer.HasEnabled.Returns(true);
            describer.IsReadOnly.Returns(false);
            describer.Requires.Returns(new RequiredFeatureAttribute[1]);
            Assert.True(composer.IsApplicableTo(describer));

            describer = Substitute.For<IFeatureDescriber>();
            describer.HasEnabled.Returns(false);
            describer.IsReadOnly.Returns(false);
            describer.Requires.Returns(new RequiredFeatureAttribute[1]);
            Assert.False(composer.IsApplicableTo(describer));

            describer = Substitute.For<IFeatureDescriber>();
            describer.HasEnabled.Returns(true);
            describer.IsReadOnly.Returns(true);
            describer.Requires.Returns(new RequiredFeatureAttribute[1]);
            Assert.False(composer.IsApplicableTo(describer));

            describer = Substitute.For<IFeatureDescriber>();
            describer.HasEnabled.Returns(true);
            describer.IsReadOnly.Returns(true);
            describer.Requires.Returns(Enumerable.Empty<RequiredFeatureAttribute>());
            Assert.False(composer.IsApplicableTo(describer));
        }

        [Fact]
        public void SetsIsEnabledIfRequirementsAreMet()
        {
            var requiredFeatureService = Substitute.For<IRequiredFeaturesService>();
            requiredFeatureService.ValidateRequiredFeatures(Arg.Any<Type>())
                .Returns(true);

            var composer = new RequiredFeatureComposer(requiredFeatureService);

            var describer = Substitute.For<IFeatureDescriber>();
            var feature = new Switch1() { IsEnabled = false };
            describer.When(x => x.SetIsEnabled<bool>(Arg.Any<object>(), Arg.Any<bool>()))
                .Do(x => feature.IsEnabled = ((bool)x.Args()[1]));

            composer.Configure(feature, describer, Substitute.For<IFeatureFactory>());
            Assert.Equal(false, feature.IsEnabled);

            feature = new Switch1() { IsEnabled = true };
            composer.Configure(feature, describer, Substitute.For<IFeatureFactory>());
            Assert.Equal(true, feature.IsEnabled);
        }

        [Fact]
        public void SetsIsEnabledIfRequirementsAreNotMet()
        {
            var requiredFeatureService = Substitute.For<IRequiredFeaturesService>();
            requiredFeatureService.ValidateRequiredFeatures(Arg.Any<Type>())
                .Returns(false);

            var composer = new RequiredFeatureComposer(requiredFeatureService);

            var describer = Substitute.For<IFeatureDescriber>();
            var feature = new Switch1() { IsEnabled = false };
            describer.When(x => x.SetIsEnabled<bool>(Arg.Any<object>(), Arg.Any<bool>()))
                .Do(x => feature.IsEnabled = ((bool)x.Args()[1]));

            composer.Configure(feature, describer, Substitute.For<IFeatureFactory>());
            Assert.Equal(false, feature.IsEnabled);

            feature = new Switch1() { IsEnabled = true };
            composer.Configure(feature, describer, Substitute.For<IFeatureFactory>());
            Assert.Equal(false, feature.IsEnabled);
        }
    }
}
