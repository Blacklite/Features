using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.OptionsModel;
using Blacklite.Framework.Features.Repositories;
using Microsoft.Framework.ConfigurationModel;
using NSubstitute;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Features.Tests.Repositories
{
    public class ConfigurationFeatureRepositoryTests
    {
        [ConfigurationFeature]
        class ConfigurationFeature : IFeature { }

        [Fact]
        public void IsOnlyApplicableToTypesWithTheProperAttribute()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureRepository(configuration);

            var describer = Substitute.For<IFeatureDescriber>();
            describer.TypeInfo.Returns(typeof(ConfigurationFeature).GetTypeInfo());
            Assert.True(composer.IsApplicableTo(describer));

            describer = Substitute.For<IFeatureDescriber>();
            describer.TypeInfo.Returns(typeof(CustomAttribute).GetTypeInfo());
            Assert.False(composer.IsApplicableTo(describer));
        }

        class CustomAttributeAttribute : Attribute { }

        [CustomAttribute]
        class CustomAttribute : IFeature { }

        [Fact]
        public void IsOnlyApplicableToTypesWithTheCustomPredicate()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureRepository(configuration, x => x.TypeInfo.GetCustomAttributes<CustomAttributeAttribute>().Any());

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
        public class TestOptions { public string Property1 { get; set; } public string Property2 { get; set; } }
        class TestFeatureOptions : Switch { public string Property1 { get; set; } public string Property2 { get; set; } }
        public class TestOptionsSwitch : Switch<TestOptions> { }
        class TestFeatureOptionsSwitch : Switch<TestFeatureOptions> { }

        [Fact]
        public void SetSwitch()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureRepository(configuration);
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

            composer.Store(
                new TestSwitch(),
                testSwitchDescriber
            );

            configuration.Received().Set($"TestSwitch:IsEnabled", Arg.Any<string>());
        }

        [Fact]
        public void SetProperties()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureRepository(configuration);
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

            composer.Store(
                new TestFeature(),
                testFeatureDescriber
            );

            configuration.Received().Set($"TestFeature:Property1", Arg.Any<string>());
            configuration.Received().Set($"TestFeature:Property2", Arg.Any<string>());

            composer.Store(
                new TestSwitchFeature(),
                testSwitchFeatureDescriber
            );

            configuration.Received().Set($"TestSwitchFeature:Property1", Arg.Any<string>());
            configuration.Received().Set($"TestSwitchFeature:Property2", Arg.Any<string>());

            composer.Store(
                new TestFeatureOptions(),
                testFeatureOptionsDescriber
            );

            configuration.Received().Set($"TestFeatureOptions:Property1", Arg.Any<string>());
            configuration.Received().Set($"TestFeatureOptions:Property2", Arg.Any<string>());
        }

        [Fact]
        public void SetNonFeatureOptions()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureRepository(configuration);
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

            var testOptionsSwitch = new TestOptionsSwitch();
            (testOptionsSwitch as IFeatureOptions).SetOptions(new TestOptions());

            composer.Store(
                testOptionsSwitch,
                testOptionsSwitchDescriber
            );

            configuration.Received().Set($"TestOptionsSwitch:Options:Property1", Arg.Any<string>());
            configuration.Received().Set($"TestOptionsSwitch:Options:Property2", Arg.Any<string>());

            composer.Store(
                new TestFeatureOptionsSwitch(),
                testFeatureOptionsSwitchDescriber
            );

            configuration.DidNotReceive().Set($"TestFeatureOptionsSwitch:Options:Property1", Arg.Any<string>());
            configuration.DidNotReceive().Set($"TestFeatureOptionsSwitch:Options:Property2", Arg.Any<string>());
        }
    }
}
