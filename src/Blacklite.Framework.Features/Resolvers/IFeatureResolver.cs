﻿
using System;

namespace Blacklite.Framework.Features.Resolvers
{
    public interface IFeatureResolver
    {
        Type GetFeatureType();
        int Priority { get; }
        bool CanResolve<T>(IFeatureResolutionContext context) where T : IFeature;
    }
}
