using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Resolvers;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Features.Tests.Resolvers
{
    public class FeatureResolverProviderTests
    {
        class Pretend : IFeature
        {
            public bool IsEnabled
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }
        }

        class Visible : IFeature
        {
            public bool IsEnabled
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }
        }

        public abstract class FeatureResolver : IFeatureResolver
        {
            public abstract int Priority { get; }

            public abstract Type GetFeatureType();

            public abstract bool CanResolve(IFeatureResolutionContext context);

            public abstract IFeature Resolve(IFeatureResolutionContext context);
        }

        [Fact]
        public void ProperlyOrganizesTypeResolvers()
        {
            var globalResolver1Mock = new Mock<FeatureResolver>();
            globalResolver1Mock.SetupGet(x => x.Priority).Returns(-100);
            globalResolver1Mock.Setup(x => x.GetFeatureType()).Returns(() => null);

            var globalResolver2Mock = new Mock<FeatureResolver>();
            globalResolver2Mock.SetupGet(x => x.Priority).Returns(100);
            globalResolver2Mock.Setup(x => x.GetFeatureType()).Returns(() => null);

            var globalResolver3Mock = new Mock<FeatureResolver>();
            globalResolver3Mock.SetupGet(x => x.Priority).Returns(1);
            globalResolver3Mock.Setup(x => x.GetFeatureType()).Returns(() => null);


            var visibleResolver1Mock = new Mock<FeatureResolver>();
            visibleResolver1Mock.SetupGet(x => x.Priority).Returns(-99);
            visibleResolver1Mock.Setup(x => x.GetFeatureType()).Returns(typeof(Visible));

            var visibleResolver2Mock = new Mock<FeatureResolver>();
            visibleResolver2Mock.SetupGet(x => x.Priority).Returns(99);
            visibleResolver2Mock.Setup(x => x.GetFeatureType()).Returns(typeof(Visible));

            var visibleResolver3Mock = new Mock<FeatureResolver>();
            visibleResolver3Mock.SetupGet(x => x.Priority).Returns(0);
            visibleResolver3Mock.Setup(x => x.GetFeatureType()).Returns(typeof(Visible));


            var pretendResolver1Mock = new Mock<FeatureResolver>();
            pretendResolver1Mock.SetupGet(x => x.Priority).Returns(99);
            pretendResolver1Mock.Setup(x => x.GetFeatureType()).Returns(typeof(Pretend));

            var pretendResolver2Mock = new Mock<FeatureResolver>();
            pretendResolver2Mock.SetupGet(x => x.Priority).Returns(-99);
            pretendResolver2Mock.Setup(x => x.GetFeatureType()).Returns(typeof(Pretend));

            var pretendResolver3Mock = new Mock<FeatureResolver>();
            pretendResolver3Mock.SetupGet(x => x.Priority).Returns(0);
            pretendResolver3Mock.Setup(x => x.GetFeatureType()).Returns(typeof(Pretend));


            var globalResolver1 = globalResolver1Mock.Object;
            var globalResolver2 = globalResolver2Mock.Object;
            var globalResolver3 = globalResolver3Mock.Object;

            var visibleResolver1 = visibleResolver1Mock.Object;
            var visibleResolver2 = visibleResolver2Mock.Object;
            var visibleResolver3 = visibleResolver3Mock.Object;

            var pretendResolver1 = pretendResolver1Mock.Object;
            var pretendResolver2 = pretendResolver2Mock.Object;
            var pretendResolver3 = pretendResolver3Mock.Object;

            var provider = new FeatureResolverProvider(
                new[] {
                    globalResolver1, globalResolver2, globalResolver3,
                    visibleResolver1, visibleResolver2, visibleResolver3,
                    pretendResolver1, pretendResolver2, pretendResolver3,
                });

            var resolvers = provider.Resolvers;

            var visibleResolvers = resolvers[typeof(Visible)].Cast<FeatureResolverDescriptor>();
            var pretendResolvers = resolvers[typeof(Pretend)].Cast<FeatureResolverDescriptor>();

            Assert.Equal(6, visibleResolvers.Count());
            Assert.Same(globalResolver2, visibleResolvers.First().Resolver);
            Assert.Same(visibleResolver2, visibleResolvers.Skip(1).First().Resolver);
            Assert.Same(globalResolver3, visibleResolvers.Skip(2).First().Resolver);
            Assert.Same(visibleResolver3, visibleResolvers.Skip(3).First().Resolver);
            Assert.Same(visibleResolver1, visibleResolvers.Skip(4).First().Resolver);
            Assert.Same(globalResolver1, visibleResolvers.Skip(5).First().Resolver);

            Assert.Equal(6, pretendResolvers.Count());
            Assert.Same(globalResolver2, pretendResolvers.First().Resolver);
            Assert.Same(pretendResolver1, pretendResolvers.Skip(1).First().Resolver);
            Assert.Same(globalResolver3, pretendResolvers.Skip(2).First().Resolver);
            Assert.Same(pretendResolver3, pretendResolvers.Skip(3).First().Resolver);
            Assert.Same(pretendResolver2, pretendResolvers.Skip(4).First().Resolver);
            Assert.Same(globalResolver1, pretendResolvers.Skip(5).First().Resolver);
        }
    }

}