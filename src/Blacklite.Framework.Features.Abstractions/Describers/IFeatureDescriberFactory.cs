using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Blacklite.Framework.Features.Describers
{
    public interface IFeatureDescriberFactory
    {
        IEnumerable<IFeatureDescriber> Create(IEnumerable<TypeInfo> descriptors);
    }
}
