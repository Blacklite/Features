using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Microsoft.Framework.DependencyInjection;
using NSubstitute;
using System;
using Xunit;

namespace Features.Tests.Factory
{
    public class CompositeFeatureFactoryTests
    {
        class Feature2 : IFeature { }

        [Fact]
        public void ReturnsSingletons()
        {
            var singletonFactory = Substitute.For<ISingletonFeatureFactory>();
            var scopedFactory = Substitute.For<IScopedFeatureFactory>();
            var describer = Substitute.For<IFeatureDescriber>();
            describer.Lifecycle.Returns(LifecycleKind.Singleton);
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();
            describerProvider.Describers[Arg.Any<Type>()].Returns(describer);


            var factory = new CompositeFeatureFactory(
                singletonFactory,
                scopedFactory,
                Substitute.For<IFeatureDescriberProvider>()
            );

            var result = factory.GetFeature(typeof(Feature2));

            singletonFactory.Received().GetFeature(Arg.Any<Type>());
            scopedFactory.DidNotReceive().GetFeature(Arg.Any<Type>());
        }

        [Fact]
        public void ReturnsScoped()
        {
            var singletonFactory = Substitute.For<ISingletonFeatureFactory>();
            var scopedFactory = Substitute.For<IScopedFeatureFactory>();
            var describer = Substitute.For<IFeatureDescriber>();
            describer.Lifecycle.Returns(LifecycleKind.Scoped);
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();
            describerProvider.Describers[Arg.Any<Type>()].Returns(describer);


            var factory = new CompositeFeatureFactory(
                singletonFactory,
                scopedFactory,
                Substitute.For<IFeatureDescriberProvider>()
            );

            var result = factory.GetFeature(typeof(Feature2));

            singletonFactory.Received().GetFeature(Arg.Any<Type>());
            scopedFactory.DidNotReceive().GetFeature(Arg.Any<Type>());
        }


    }
}
