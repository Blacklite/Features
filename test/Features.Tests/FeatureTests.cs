using Blacklite.Framework.Features;
using System;
using Xunit;

namespace Features.Tests
{
    public class FeatureTests
    {
        private class AlwaysOn : Feature.AlwaysOn { }
        private class AlwaysOff : Feature.AlwaysOff { }

        [Fact]
        public void AlwaysOnFeatureReturnsTrue()
        {
            var value = new AlwaysOn();
            Assert.True(value.IsEnabled);
        }

        [Fact]
        public void AlwaysOffFeatureReturnsFalse()
        {
            var value = new AlwaysOff();
            Assert.False(value.IsEnabled);
        }
    }
}
