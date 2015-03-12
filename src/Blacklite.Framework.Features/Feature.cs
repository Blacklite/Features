using System;
using System.Collections.Generic;
using Microsoft.Framework.DependencyInjection;
using System.Linq;
using Blacklite.Framework.Features.OptionModel;
using System.ComponentModel.DataAnnotations;

namespace Blacklite.Framework.Features
{
    public abstract class Aspect : IAspect
    {

    }

    public abstract class Aspect<TOptions> : Aspect, IAspect<TOptions>
        where TOptions : class, new()
    {
        public TOptions Options { get; }

        public Aspect(IAspectOptions<TOptions> _optionsContainer)
        {
            Options = _optionsContainer.Options;
        }
    }

    public abstract partial class Feature : IFeature, IDisposable
    {
        private readonly IRequiredFeaturesService _requiredFeatures;
        private IValidateFeatureService _validateFeatureService;

        public Feature(IRequiredFeaturesService requiredFeatures)
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

    public abstract class Feature<TOptions> : Feature, IFeature<TOptions>
        where TOptions : class, new()
    {
        public TOptions Options { get; }

        public Feature(IRequiredFeaturesService requiredFeatures, IAspectOptions<TOptions> _optionsContainer) : base(requiredFeatures)
        {
            Options = _optionsContainer.Options;
        }
    }
}
