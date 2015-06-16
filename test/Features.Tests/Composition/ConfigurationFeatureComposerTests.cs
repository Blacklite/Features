using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Blacklite.Framework.Features.OptionsModel;
using Microsoft.Framework.Configuration;
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
        class ConfigurationFeature : IFeature { }

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

        class CustomAttributeAttribute : Attribute { }

        [CustomAttribute]
        class CustomAttribute : IFeature { }

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

        class AlwaysOnSwitch : Feature.AlwaysOn { }
        class AlwaysOffSwitch : Feature.AlwaysOff { }

        [Fact]
        public void SetSwitch()
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

            string value;
            var testSwitchDescriber = describers.Single(x => x.Type == typeof(TestSwitch));

            composer.Configure(
                new TestSwitch(),
                testSwitchDescriber,
                Substitute.For<IFeatureFactory>()
            );

            configuration.Received().TryGet($"TestSwitch:IsEnabled", out value);
        }

        [Fact]
        public void NotSetReadOnlySwitch()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureComposer(configuration);
            var factory = new FeatureDescriberFactory();
            var describers = factory.Create(new Type[]
            {
                typeof(AlwaysOnSwitch),
                typeof(AlwaysOffSwitch),
                typeof(TestSwitchFeature)
            }.Select(IntrospectionExtensions.GetTypeInfo)
            .ToArray());

            string value;
            var describer = describers.Single(x => x.Type == typeof(TestSwitchFeature));
            composer.Configure(
                new TestSwitchFeature(),
                describer,
                Substitute.For<IFeatureFactory>()
            );
            configuration.Received().TryGet($"TestSwitchFeature:IsEnabled", out value);

            describer = describers.Single(x => x.Type == typeof(AlwaysOnSwitch));
            composer.Configure(
                new AlwaysOnSwitch(),
                describer,
                Substitute.For<IFeatureFactory>()
            );
            configuration.DidNotReceive().TryGet($"AlwaysOnSwitch:IsEnabled", out value);

            describer = describers.Single(x => x.Type == typeof(AlwaysOffSwitch));
            composer.Configure(
                new AlwaysOffSwitch(),
                describer,
                Substitute.For<IFeatureFactory>()
            );
            configuration.DidNotReceive().TryGet($"AlwaysOffSwitch:IsEnabled", out value);
        }
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
                testSwitchDescriber,
                Substitute.For<IFeatureFactory>()
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
                testFeatureDescriber,
                Substitute.For<IFeatureFactory>()
            );

            configuration.Received().TryGet($"TestFeature:Property1", out value);
            configuration.Received().TryGet($"TestFeature:Property2", out value);

            composer.Configure(
                new TestSwitchFeature(),
                testSwitchFeatureDescriber,
                Substitute.For<IFeatureFactory>()
            );

            configuration.Received().TryGet($"TestSwitchFeature:Property1", out value);
            configuration.Received().TryGet($"TestSwitchFeature:Property2", out value);

            composer.Configure(
                new TestFeatureOptions(),
                testFeatureOptionsDescriber,
                Substitute.For<IFeatureFactory>()
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
                testOptionsSwitchDescriber,
                Substitute.For<IFeatureFactory>()
            );

            configuration.Received().TryGet($"TestOptionsSwitch:Options:Property1", out value);
            configuration.Received().TryGet($"TestOptionsSwitch:Options:Property2", out value);

            composer.Configure(
                new TestFeatureOptionsSwitch(),
                testFeatureOptionsSwitchDescriber,
                Substitute.For<IFeatureFactory>()
            );

            configuration.DidNotReceive().TryGet($"TestFeatureOptionsSwitch:Options:Property1", out value);
            configuration.DidNotReceive().TryGet($"TestFeatureOptionsSwitch:Options:Property2", out value);
        }

        public class ReadOnlyOptions { public string Property1 { get; } public string Property2 { get; set; } }
        public class ReadOnlyFeatureOptions : Switch { public string Property1 { get; set; } public string Property2 { get; } }
        public class ReadOnlyOptionsSwitch : Switch<ReadOnlyOptions> { }

        [Fact]
        public void HandlesReadOnlyProperties()
        {
            var configuration = Substitute.For<IConfiguration>();

            var composer = new ConfigurationFeatureComposer(configuration);
            var factory = new FeatureDescriberFactory();
            var describers = factory.Create(new Type[]
            {
                typeof(ReadOnlyOptions),
                typeof(ReadOnlyFeatureOptions),
                typeof(ReadOnlyOptionsSwitch),
            }.Select(IntrospectionExtensions.GetTypeInfo)
            .ToArray());

            string value;
            var describer = describers.Single(x => x.Type == typeof(ReadOnlyOptionsSwitch));

            var testOptionsSwitch = new ReadOnlyOptionsSwitch();
            (testOptionsSwitch as IFeatureOptions).SetOptions(new ReadOnlyOptions());

            composer.Configure(
                testOptionsSwitch,
                describer,
                Substitute.For<IFeatureFactory>()
            );

            configuration.DidNotReceive().TryGet($"ReadOnlyOptionsSwitch:Options:Property1", out value);
            configuration.Received().TryGet($"ReadOnlyOptionsSwitch:Options:Property2", out value);

            describer = describers.Single(x => x.Type == typeof(ReadOnlyFeatureOptions));
            composer.Configure(
                new ReadOnlyFeatureOptions(),
                describer,
                Substitute.For<IFeatureFactory>()
            );

            configuration.Received().TryGet($"ReadOnlyFeatureOptions:Property1", out value);
            configuration.DidNotReceive().TryGet($"ReadOnlyFeatureOptions:Property2", out value);
        }

        [Fact]
        public void HandlesReadOnlyOptionProperties()
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

            string value;
            var testOptionsSwitchDescriber = describers.Single(x => x.Type == typeof(TestOptionsSwitch));
            var testFeatureOptionsSwitchDescriber = describers.Single(x => x.Type == typeof(TestFeatureOptionsSwitch));

            var testOptionsSwitch = new TestOptionsSwitch();
            (testOptionsSwitch as IFeatureOptions).SetOptions(new TestOptions());

            composer.Configure(
                testOptionsSwitch,
                testOptionsSwitchDescriber,
                Substitute.For<IFeatureFactory>()
            );

            configuration.Received().TryGet($"TestOptionsSwitch:Options:Property1", out value);
            configuration.Received().TryGet($"TestOptionsSwitch:Options:Property2", out value);

            composer.Configure(
                new TestFeatureOptionsSwitch(),
                testFeatureOptionsSwitchDescriber,
                Substitute.For<IFeatureFactory>()
            );

            configuration.DidNotReceive().TryGet($"TestFeatureOptionsSwitch:Options:Property1", out value);
            configuration.DidNotReceive().TryGet($"TestFeatureOptionsSwitch:Options:Property2", out value);
        }
    }
}
