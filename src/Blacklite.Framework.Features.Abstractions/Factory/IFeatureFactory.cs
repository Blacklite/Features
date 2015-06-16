using System;

namespace Blacklite.Framework.Features.Factory
{
    public interface IFeatureFactory
    {
        IFeature GetFeature(Type featureType);
    }
}
