using System;

namespace Blacklite.Framework.Features.Stores
{
    public interface IFeatureStoreProvider
    {
        IFeatureStore GetFeatureStore(IFeatureDescriber describer);
    }
}
