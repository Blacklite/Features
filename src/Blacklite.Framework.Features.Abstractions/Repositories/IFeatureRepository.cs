﻿using Blacklite.Framework.Features.Describers;
using System;

namespace Blacklite.Framework.Features.Repositories
{
    public interface IFeatureRepository
    {
        int Priority { get; }

        bool IsApplicableTo(IFeatureDescriber describer);

        void Store(IFeature feature, IFeatureDescriber describer);
    }
}
