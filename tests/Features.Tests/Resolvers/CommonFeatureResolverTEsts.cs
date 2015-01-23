using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Resolvers;
using System;
using Xunit;

namespace Features.Tests.Resolvers
{
    public class CommonFeatureResolverTests
    {
        private class Custom : Feature { }

        [Fact]
        public void CanResolverForTypesDefinedInFeature()
        {
            var resolver = new CommonFeatureResolver();

            Assert.True(resolver.CanResolve(new FeatureResolutionContext(null, typeof(Feature.AlwaysOff))));
            Assert.True(resolver.CanResolve(new FeatureResolutionContext(null, typeof(Feature.AlwaysOn))));
            Assert.True(resolver.CanResolve(new FeatureResolutionContext(null, typeof(Feature.Random))));
            Assert.False(resolver.CanResolve(new FeatureResolutionContext(null, typeof(Custom))));
        }
    }
}