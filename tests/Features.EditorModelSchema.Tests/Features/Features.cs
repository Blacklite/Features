﻿using Blacklite.Framework.Features;
using System;
using Blacklite.Framework.Features.OptionModel;
using Microsoft.Framework.DependencyInjection;

namespace Features.EditorModelSchema.Tests.Features
{
    public static class FeatureRegistry
    {
        public static void Register(IServiceCollection services)
        {
            services.AddTransient<FeatureA>();
            services.AddTransient<FeatureB>();
            services.AddTransient<FeatureC>();
            services.AddTransient<FeatureD>();
        }
    }
    public class FeatureA : Feature.AlwaysOn
    {

    }

    public class FeatureB : Feature.AlwaysOff
    {

    }

    public enum CeeOptions
    {
        Private,
        Public,
        Protected
    }

    public class FeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Enabled { get; set; }
        public CeeOptions Options { get; }
    }

    public class FeatureC : Feature.AlwaysOn<FeatureCeeOptions>
    {
        public FeatureC(IFeatureOptions<FeatureCeeOptions> _optionsContainer) : base(_optionsContainer)
        {
        }
    }

    [ParentFeature(typeof(FeatureA))]
    public class FeatureD : Feature
    {
        public FeatureD(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures)
        {
        }
    }
}
