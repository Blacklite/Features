using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public interface IFeature<T>
        where T : IFeature
    {
        T Value { get; }
    }

    public class Feature<T> : IFeature<T>
        where T : IFeature
    {
        public Feature(IFeatureProvider featureProvider)
        {
            Value = featureProvider.GetFeature<T>();
        }

        public T Value { get; private set; }
    }
}
