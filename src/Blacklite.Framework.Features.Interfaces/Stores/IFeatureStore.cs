using System;

namespace Blacklite.Framework.Features.Stores
{
    public interface IFeatureStore
    {
        int Priority { get; }

        bool CanStore(IFeatureDescriber describer);

        void Store(ISwitch feature, IFeatureDescriber describer);
    }
}
