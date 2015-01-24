using System;
using System.Collections.Generic;
using Microsoft.Framework.DependencyInjection;
using System.Linq;

namespace Blacklite.Framework.Features
{
    public interface IFeature
    {
        bool IsEnabled { get; }
    }

    public abstract partial class Feature : IFeature, IDisposable
    {
        private readonly IRequiredFeaturesService _requiredFeatures;
        private IValidateFeatureService _validateFeatureService;

        protected Feature(IRequiredFeaturesService requiredFeatures)
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
            ((IDisposable)_validateFeatureService).Dispose();
        }
    }
}
