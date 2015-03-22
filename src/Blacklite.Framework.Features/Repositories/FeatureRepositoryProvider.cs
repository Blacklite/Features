using System;
using Blacklite.Framework.Features.Describers;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.Repositories
{
    public class FeatureRepositoryProvider : IFeatureRepositoryProvider
    {
        private readonly IEnumerable<IFeatureRepository> _repositories;

        public FeatureRepositoryProvider(IEnumerable<IFeatureRepository> repositories)
        {
            _repositories = repositories.OrderByDescending(x => x.Priority).ToArray();
        }

        public IFeatureRepository GetFeatureRepository(IFeatureDescriber describer)
        {
            return _repositories.FirstOrDefault(x => x.CanStore(describer));
        }
    }
}
