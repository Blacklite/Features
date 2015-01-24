using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    class FeatureServicesCollection
    {
        public FeatureServicesCollection(IServiceCollection collection)
        {
            Descriptors = collection;
        }

        public IEnumerable<IServiceDescriptor> Descriptors { get; }
    }
}
