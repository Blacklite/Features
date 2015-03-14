using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Blacklite.Framework.Features.Describers
{
    public interface IFeatureDescriberFactory
    {
        IEnumerable<IFeatureDescriber> Create(IEnumerable<IServiceDescriptor> descriptors);
    }
}
