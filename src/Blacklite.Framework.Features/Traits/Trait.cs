using Blacklite.Framework.Features.OptionModel;
using System;

namespace Blacklite.Framework.Features.Traits
{
    public abstract partial class Trait : ITrait, IDisposable
    {
        private readonly IRequiredFeaturesService _requiredFeatures;
        private IValidateFeatureService _validateFeatureService;

        public Trait(IRequiredFeaturesService requiredFeatures)
        {
            _requiredFeatures = requiredFeatures;
        }

        private bool _enabled = true;
        public virtual bool IsEnabled
        {
            get
            {
                if (_validateFeatureService == null)
                    _validateFeatureService = _requiredFeatures.ValidateFeaturesAreInTheCorrectState(this.GetType());

                return _enabled && _validateFeatureService.Validate();
            }
            set { _enabled = value; }
        }

        public void Dispose()
        {
            _validateFeatureService.Dispose();
        }
    }

    public abstract class Trait<TOptions> : Trait, ITrait<TOptions>
        where TOptions : class, new()
    {
        public TOptions Options { get; }

        public Trait(IRequiredFeaturesService requiredFeatures, IAspectOptions<TOptions> _optionsContainer) : base(requiredFeatures)
        {
            Options = _optionsContainer.Options;
        }
    }
}
