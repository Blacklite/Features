using Blacklite.Framework.Features.Traits;
using System;

namespace Blacklite.Framework.Features.Stores
{
    public interface IFeatureStore
    {
        int Priority { get; }

        bool CanStore(IFeatureDescriber describer);

        void Store(ITrait feature, IFeatureDescriber describer);
    }
}
