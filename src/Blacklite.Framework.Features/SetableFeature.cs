using System;
using System.Collections.Generic;
using Microsoft.Framework.DependencyInjection;
using System.Linq;
using Blacklite.Framework.Features.OptionModel;

namespace Blacklite.Framework.Features
{

    public abstract class SetableFeature : IFeature, IDisposable
    {
        private readonly IRequiredFeaturesService _requiredFeatures;
        private IValidateFeatureService _validateFeatureService;

        protected SetableFeature(IRequiredFeaturesService requiredFeatures)
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

    public abstract class SetableFeature<TOptions> : SetableFeature, IFeature<TOptions>
        where TOptions : class, new()
    {
        public TOptions Options { get; }

        protected SetableFeature(IRequiredFeaturesService requiredFeatures, IFeatureOptions<TOptions> _optionsContainer) : base(requiredFeatures)
        {
            Options = _optionsContainer.Options;
        }
    }
}