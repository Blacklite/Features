using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Observables;
using NSubstitute;
using System;
using Xunit;

namespace Features.Tests.Observables
{
    public class FeatureSubjectFactoryTests
    {
        public class Feature2 : IObservableFeature { }

        [Fact]
        public void GetFeatureCachesInstances()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();

            var factory = new FeatureSubjectFactory(serviceProvider);

            var result = factory.GetSubject(typeof(Feature2));

            var result2 = factory.GetSubject(typeof(Feature2));

            Assert.Same(result, result2);
        }
    }
}
