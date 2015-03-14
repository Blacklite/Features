using Blacklite.Framework.Features.Describers;
using System;

namespace Blacklite.Framework.Features.Repositories
{
    public interface IFeatureRepositoryProvider
    {
        IFeatureRepository GetFeatureRepository(IFeatureDescriber describer);
    }
}
