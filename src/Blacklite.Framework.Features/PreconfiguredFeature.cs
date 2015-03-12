﻿using System;
using System.Collections.Generic;
using Microsoft.Framework.DependencyInjection;
using System.Linq;
using Blacklite.Framework.Features.OptionModel;
using System.ComponentModel.DataAnnotations;
using Blacklite.Framework.Features.Traits;

namespace Blacklite.Framework.Features
{
    public abstract class PreconfiguredFeature : ITrait, IDisposable
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

    public abstract class PreconfiguredFeature<TOptions> : PreconfiguredFeature, ITrait<TOptions>
        where TOptions : class, new()
    {
        public TOptions Options { get; }

        public PreconfiguredFeature(IRequiredFeaturesService requiredFeatures, IAspectOptions<TOptions> _optionsContainer) : base(requiredFeatures)
        {
            Options = _optionsContainer.Options;
        }
    }
}
