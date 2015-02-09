using System;
using System.Collections.Generic;
using Microsoft.Framework.DependencyInjection;
using System.Linq;
using Blacklite.Framework.Features.OptionModel;
using System.ComponentModel.DataAnnotations;

namespace Blacklite.Framework.Features
{
    public interface IFeature
    {
        [Display(Name ="On")]
        bool IsEnabled { get; }
    }

    public interface IFeature<TOptions> : IFeature, IFeatureOptions where TOptions : class, new()
    {
        TOptions Options { get; }
    }

    public abstract class PreconfiguredFeature : IFeature, IDisposable
    {
        private readonly IRequiredFeaturesService _requiredFeatures;
        private IValidateFeatureService _validateFeatureService;

        public PreconfiguredFeature(IRequiredFeaturesService requiredFeatures)
        {
            _requiredFeatures = requiredFeatures;
        }

        public virtual bool IsEnabled
        {
            get
            {
                if (_validateFeatureService == null)
                    _validateFeatureService = _requiredFeatures.ValidateFeaturesAreInTheCorrectState(this.GetType());

                return _validateFeatureService.Validate();
            }
        }

        public void Dispose()
        {
            _validateFeatureService.Dispose();
        }
    }

    public abstract class PreconfiguredFeature<TOptions> : PreconfiguredFeature, IFeature<TOptions>
        where TOptions : class, new()
    {
        public TOptions Options { get; }

        public PreconfiguredFeature(IRequiredFeaturesService requiredFeatures, IFeatureOptions<TOptions> _optionsContainer) : base(requiredFeatures)
        {
            Options = _optionsContainer.Options;
        }
    }
}
