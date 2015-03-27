using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Microsoft.Framework.ConfigurationModel;
using NSubstitute;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Features.Tests.Composition
{
    public class ConfigurationFeatureComposerTests
    {
        [ConfigurationFeature]
        class ConfigurationFeature : IFeature
        {

        }

        [Fact]
        public void IsOnlyApplicableToTypesWithTheProperAttribute()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureComposer(configuration);

            var describer = Substitute.For<IFeatureDescriber>();
            describer.TypeInfo.Returns(typeof(ConfigurationFeature).GetTypeInfo());
            Assert.True(composer.IsApplicableTo(describer));

            describer = Substitute.For<IFeatureDescriber>();
            describer.TypeInfo.Returns(typeof(CustomAttribute).GetTypeInfo());
            Assert.False(composer.IsApplicableTo(describer));
        }

        class CustomAttributeAttribute : Attribute
        {

        }

        [CustomAttribute]
        class CustomAttribute : IFeature
        {

        }

        [Fact]
        public void IsOnlyApplicableToTypesWithTheCustomPredicate()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureComposer(configuration, x => x.TypeInfo.GetCustomAttributes<CustomAttributeAttribute>().Any());

            var describer = Substitute.For<IFeatureDescriber>();
            describer.TypeInfo.Returns(typeof(ConfigurationFeature).GetTypeInfo());
            Assert.False(composer.IsApplicableTo(describer));

            describer = Substitute.For<IFeatureDescriber>();
            describer.TypeInfo.Returns(typeof(CustomAttribute).GetTypeInfo());
            Assert.True(composer.IsApplicableTo(describer));
        }

        class TestSwitch : Switch { }
        class TestFeature : Feature { public string Property1 { get; set; } public string Property2 { get; set; } }
        class TestSwitchFeature : Switch { public string Property1 { get; set; } public string Property2 { get; set; } }
        class TestOptions { public string Property1 { get; set; } public string Property2 { get; set; } }
        class TestFeatureOptions : Switch { public string Property1 { get; set; } public string Property2 { get; set; } }
        class TestOptionsSwitch : Switch<TestOptions> { }
        class TestFeatureOptionsSwitch : Switch<TestFeatureOptions> { }

        [Fact]
        public void TryGetSwitch()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureComposer(configuration);
            var factory = new FeatureDescriberFactory();
            var describers = factory.Create(new Type[]
            {
                typeof(TestSwitch),
                typeof(TestFeature),
                typeof(TestSwitchFeature),
                typeof(TestFeatureOptions),
                typeof(TestOptionsSwitch),
                typeof(TestFeatureOptionsSwitch),
            }.Select(IntrospectionExtensions.GetTypeInfo)
            .ToArray());

            var testSwitchDescriber = describers.Single(x => x.Type == typeof(TestSwitch));
            string value;

            composer.Configure(
                new TestSwitch(),
                testSwitchDescriber
            );

            configuration.Received().TryGet($"TestSwitch:IsEnabled", out value);
        }

        [Fact]
        public void TryGetProperties()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureComposer(configuration);
            var factory = new FeatureDescriberFactory();
            var describers = factory.Create(new Type[]
            {
                typeof(TestSwitch),
                typeof(TestFeature),
                typeof(TestSwitchFeature),
                typeof(TestFeatureOptions),
                typeof(TestOptionsSwitch),
                typeof(TestFeatureOptionsSwitch),
            }.Select(IntrospectionExtensions.GetTypeInfo)
            .ToArray());

            var testFeatureDescriber = describers.Single(x => x.Type == typeof(TestFeature));
            var testSwitchFeatureDescriber = describers.Single(x => x.Type == typeof(TestSwitchFeature));
            var testFeatureOptionsDescriber = describers.Single(x => x.Type == typeof(TestFeatureOptions));
            string value;

            composer.Configure(
                new TestFeature(),
                testFeatureDescriber
            );

            configuration.Received().TryGet($"TestFeature:Property1", out value);
            configuration.Received().TryGet($"TestFeature:Property2", out value);

            composer.Configure(
                new TestSwitchFeature(),
                testSwitchFeatureDescriber
            );

            configuration.Received().TryGet($"TestSwitchFeature:Property1", out value);
            configuration.Received().TryGet($"TestSwitchFeature:Property2", out value);

            composer.Configure(
                new TestFeatureOptions(),
                testFeatureOptionsDescriber
            );

            configuration.Received().TryGet($"TestFeatureOptions:Property1", out value);
            configuration.Received().TryGet($"TestFeatureOptions:Property2", out value);
        }

        [Fact]
        public void TryGetNonFeatureOptions()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureComposer(configuration);
            var factory = new FeatureDescriberFactory();
            var describers = factory.Create(new Type[]
            {
                typeof(TestSwitch),
                typeof(TestFeature),
                typeof(TestSwitchFeature),
                typeof(TestFeatureOptions),
                typeof(TestOptionsSwitch),
                typeof(TestFeatureOptionsSwitch),
            }.Select(IntrospectionExtensions.GetTypeInfo)
            .ToArray());

            var testOptionsSwitchDescriber = describers.Single(x => x.Type == typeof(TestOptionsSwitch));
            var testFeatureOptionsSwitchDescriber = describers.Single(x => x.Type == typeof(TestFeatureOptionsSwitch));
            string value;

            composer.Configure(
                new TestOptionsSwitch(),
                testOptionsSwitchDescriber
            );

            configuration.Received().TryGet($"TestOptionsSwitch:Options:Property1", out value);
            configuration.Received().TryGet($"TestOptionsSwitch:Options:Property2", out value);

            composer.Configure(
                new TestFeatureOptionsSwitch(),
                testFeatureOptionsSwitchDescriber
            );

            configuration.DidNotReceive().TryGet($"TestFeatureOptionsSwitch:Options:Property1", out value);
            configuration.DidNotReceive().TryGet($"TestFeatureOptionsSwitch:Options:Property2", out value);
        }
    }
}
