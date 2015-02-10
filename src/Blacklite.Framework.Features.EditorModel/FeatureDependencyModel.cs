using System;

namespace Blacklite.Framework.Features.EditorModel
{
    public class FeatureDependencyModel
    {
        public FeatureDependencyModel(IFeatureDescriber describer, bool isEnabled)
        {
            Feature = describer;
            IsEnabled = isEnabled;
        }

        public IFeatureDescriber Feature { get; }
        public bool IsEnabled { get; }
    }
}
