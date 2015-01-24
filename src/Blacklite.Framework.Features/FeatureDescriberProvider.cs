using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    public interface IFeatureDescriberProvider
    {
        IReadOnlyDictionary<Type, IFeatureDescriber> Describers { get; }
    }

    class FeatureDescriberProvider : IFeatureDescriberProvider
    {
        //private readonly IEnumerable<IServiceDescriptor> _describers;
        public FeatureDescriberProvider(FeatureServicesCollection collection, IFeatureDescriberFactory factory)
        {
            var dictionary = factory.Create(
                    collection.Descriptors
                        .Where(x => x.ServiceType
                            .GetTypeInfo()
                            .ImplementedInterfaces.Contains(typeof(IFeature))
                        ))
                        .ToDictionary(x => x.FeatureType);

            Describers = new ReadOnlyDictionary<Type, IFeatureDescriber>(dictionary);
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<Type, IFeatureDescriber> Describers { get; }
    }
}
