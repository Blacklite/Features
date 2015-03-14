using Blacklite.Framework.Features.OptionsModel;
using System;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Setups
{
    public class RequiredAspectSetup : IFeatureSetup
    {
        private readonly IRequiredFeaturesService _requiredFeatures;
        private readonly IFeatureDescriberProvider _describerProvider;
        public RequiredAspectSetup(IRequiredFeaturesService requiredFeatures, IFeatureDescriberProvider describerProvider)
        {
            _requiredFeatures = requiredFeatures;
            _describerProvider = describerProvider;
        }

        public int Priority { get; } = 0;

        public T Configure<T>(T aspect)
        {
            var trait = aspect as ISwitch;
            var describer = _describerProvider.Describers[typeof(T)];

            var value = _requiredFeatures.ValidateRequiredFeatures(describer.FeatureType);
            describer.SetIsEnabled(aspect, trait.IsEnabled && value);

            return aspect;
        }

        public bool IsApplicableTo(IFeatureDescriber describer)
        {
            return describer.HasEnabled && !describer.IsReadOnly && describer.Requires.Any();
        }
    }
}
