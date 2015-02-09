using Blacklite.Framework.Features;
using System;
using Blacklite.Framework.Features.OptionModel;

namespace Features.EditorModelSchema.Tests.Features
{
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
        protected FeatureC(IFeatureOptions<FeatureCeeOptions> _optionsContainer) : base(_optionsContainer)
        {
        }
    }
}