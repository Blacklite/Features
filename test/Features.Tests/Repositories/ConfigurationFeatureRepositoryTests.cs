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
        class AlwaysOnSwitch : Feature.AlwaysOn { }
        class AlwaysOffSwitch : Feature.AlwaysOff { }

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
        public void NotSetReadOnlySwitch()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureRepository(configuration);
            var factory = new FeatureDescriberFactory();
            var describers = factory.Create(new Type[]
            {
                typeof(AlwaysOnSwitch),
                typeof(AlwaysOffSwitch),
                typeof(TestSwitchFeature)
            }.Select(IntrospectionExtensions.GetTypeInfo)
            .ToArray());

            var describer = describers.Single(x => x.Type == typeof(TestSwitchFeature));
            composer.Store(
                new TestSwitchFeature(),
                describer
            );
            configuration.Received().Set($"TestSwitchFeature:IsEnabled", Arg.Any<string>());

            describer = describers.Single(x => x.Type == typeof(AlwaysOnSwitch));
            composer.Store(
                new AlwaysOnSwitch(),
                describer
            );
            configuration.DidNotReceive().Set($"AlwaysOnSwitch:IsEnabled", Arg.Any<string>());

            describer = describers.Single(x => x.Type == typeof(AlwaysOffSwitch));
            composer.Store(
                new AlwaysOffSwitch(),
                describer
            );
            configuration.DidNotReceive().Set($"AlwaysOffSwitch:IsEnabled", Arg.Any<string>());
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

        public class ReadOnlyOptions { public string Property1 { get; } public string Property2 { get; set; } }
        public class ReadOnlyFeatureOptions : Switch { public string Property1 { get; set; } public string Property2 { get; } }
        public class ReadOnlyOptionsSwitch : Switch<ReadOnlyOptions> { }


        [Fact]
        public void HandlesReadOnlyProperties()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureRepository(configuration);
            var factory = new FeatureDescriberFactory();
            var describers = factory.Create(new Type[]
            {
                typeof(ReadOnlyOptions),
                typeof(ReadOnlyFeatureOptions),
                typeof(ReadOnlyOptionsSwitch),
            }.Select(IntrospectionExtensions.GetTypeInfo)
            .ToArray());

            var describer = describers.Single(x => x.Type == typeof(ReadOnlyOptionsSwitch));

            var testOptionsSwitch = new ReadOnlyOptionsSwitch();
            (testOptionsSwitch as IFeatureOptions).SetOptions(new ReadOnlyOptions());

            composer.Store(
                testOptionsSwitch,
                describer
            );

            configuration.DidNotReceive().Set($"ReadOnlyOptionsSwitch:Options:Property1", Arg.Any<string>());
            configuration.Received().Set($"ReadOnlyOptionsSwitch:Options:Property2", Arg.Any<string>());

            describer = describers.Single(x => x.Type == typeof(ReadOnlyFeatureOptions));
            composer.Store(
                new ReadOnlyFeatureOptions(),
                describer
            );

            configuration.Received().Set($"ReadOnlyFeatureOptions:Property1", Arg.Any<string>());
            configuration.DidNotReceive().Set($"ReadOnlyFeatureOptions:Property2", Arg.Any<string>());
        }

        [Fact]
        public void HandlesReadOnlyOptionProperties()
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
