using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public interface IFeature
    {
        bool IsEnabled { get; }
        IEnumerable<IFeature> DependsOn { get; }
        IEnumerable<IFeature> Contains { get; }
    }

    public class Feature<T> where T : IFeature
    {
        public Feature(IFeatureProvider featureProvider)
        {
            Configuration = featureProvider.GetFeature<T>();
            Changed = featureProvider.GetChangedStream<T>();
        }

        public T Configuration { get; }

        public bool IsEnabled { get { return Configuration.IsEnabled; } }

        public IObservable<T> Changed { get; }
    }
}