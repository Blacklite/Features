using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Blacklite.Framework.Features.Observables;
using NSubstitute;
using System;
using Xunit;

namespace Features.Tests.Observables
{
    public class FeatureSubjectTests
    {
        public class Feature2 : IFeature, IObservableFeature { }

        [Fact]
        public void HandlesRequiredFeatures()
        {
            var featureValue = Substitute.For<Feature2>();
            var feature = Substitute.For<Feature<Feature2>>();
            feature.Value.Returns(featureValue);
            var featureFactory = Substitute.For<IFeatureFactory>();
            featureFactory.GetFeature(typeof(Feature2)).Returns(featureValue);

            var boolObservable = Substitute.For<IObservable<bool>>();
            var requiredFeatureService = Substitute.For<IRequiredFeaturesService>();
            requiredFeatureService.GetObservableRequiredFeatures(typeof(Feature2)).Returns(boolObservable);

            var featureDescriber = Substitute.For<IFeatureDescriber>();
            var featureDescriberProvider = Substitute.For<IFeatureDescriberProvider>();
            featureDescriberProvider.Describers[typeof(Feature2)].Returns(featureDescriber);

            var featureSubjectFactory = Substitute.For<IFeatureSubjectFactory>();

            var subject = new FeatureSubject<Feature2>(
                feature,
                featureFactory,
                requiredFeatureService,
                featureDescriberProvider,
                featureSubjectFactory
            );

            boolObservable.ReceivedWithAnyArgs().Subscribe();
        }

        public class FeatureOptions : ObservableFeature { }
        public class Feature3 : ObservableSwitch<FeatureOptions> { }

        [Fact]
        public void SupportsObservableFeatures()
        {
            var featureValue = Substitute.For<Feature3>();
            var feature = Substitute.For<Feature<Feature3>>();
            feature.Value.Returns(featureValue);
            var featureFactory = Substitute.For<IFeatureFactory>();
            featureFactory.GetFeature(typeof(Feature3)).Returns(featureValue);

            var requiredFeatureService = Substitute.For<IRequiredFeaturesService>();

            var featureDescriber = Substitute.For<IFeatureDescriber>();
            featureDescriber.HasOptions.Returns(true);

            var optionsDescriber = Substitute.For<IFeatureOptionsDescriber>();
            featureDescriber.Options.Returns(optionsDescriber);
            optionsDescriber.IsFeature.Returns(true);
            optionsDescriber.Type.Returns(typeof(FeatureOptions));

            var optionsFeatureDescriber = Substitute.For<IFeatureDescriber>();
            optionsFeatureDescriber.IsObservable.Returns(true);
            optionsFeatureDescriber.Type.Returns(typeof(FeatureOptions));

            var featureDescriberProvider = Substitute.For<IFeatureDescriberProvider>();
            featureDescriberProvider.Describers[typeof(Feature3)].Returns(featureDescriber);
            featureDescriberProvider.Describers[typeof(FeatureOptions)].Returns(optionsFeatureDescriber);

            var featureSubject = Substitute.For<IFeatureSubject>();

            var featureSubjectFactory = Substitute.For<IFeatureSubjectFactory>();
            featureSubjectFactory.GetSubject(typeof(FeatureOptions)).Returns(featureSubject);

            var subject = new FeatureSubject<Feature3>(
                feature,
                featureFactory,
                requiredFeatureService,
                featureDescriberProvider,
                featureSubjectFactory
            );

            featureSubjectFactory.Received().GetSubject(typeof(FeatureOptions));
        }
    }
}
