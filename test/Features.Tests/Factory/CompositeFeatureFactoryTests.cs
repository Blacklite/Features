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
            describer.IsObservable.Returns(true);
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();
            describerProvider.Describers[typeof(Feature2)].Returns(describer);


            var factory = new CompositeFeatureFactory(
                singletonFactory,
                scopedFactory,
                describerProvider
            );

            var result = factory.GetFeature(typeof(Feature2));

            singletonFactory.Received().GetFeature(typeof(Feature2));
            scopedFactory.DidNotReceive().GetFeature(typeof(Feature2));
        }

        [Fact]
        public void ReturnsScoped()
        {
            var singletonFactory = Substitute.For<ISingletonFeatureFactory>();
            var scopedFactory = Substitute.For<IScopedFeatureFactory>();
            var describer = Substitute.For<IFeatureDescriber>();
            describer.IsObservable.Returns(false);
            var describerProvider = Substitute.For<IFeatureDescriberProvider>();
            describerProvider.Describers[typeof(Feature2)].Returns(describer);


            var factory = new CompositeFeatureFactory(
                singletonFactory,
                scopedFactory,
                describerProvider
            );

            var result = factory.GetFeature(typeof(Feature2));

            scopedFactory.Received().GetFeature(typeof(Feature2));
            singletonFactory.DidNotReceive().GetFeature(typeof(Feature2));
        }


    }
}
