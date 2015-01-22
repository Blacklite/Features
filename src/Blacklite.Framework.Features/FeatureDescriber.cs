using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public interface IFeatureDescriber
    {
        IFeature Feature { get; }
        IEnumerable<IFeatureDependencyDescriber> DependsOn { get; }
        IEnumerable<IFeatureDependencyDescriber> Contains { get; }
    }

    class FeatureDescriber : IFeatureDescriber
    {
        public IFeature Feature { get; }
        public IEnumerable<IFeatureDependencyDescriber> DependsOn { get; }
        public IEnumerable<IFeatureDependencyDescriber> Contains { get; }
    }

    public interface IFeatureDependencyDescriber
    {
        IFeature Feature { get; }
        bool IsEnabled { get; }
    }

    class FeatureDependencyDescriber : IFeatureDependencyDescriber
    {
        public IFeature Feature { get; }
        public bool IsEnabled { get; }
    }
}