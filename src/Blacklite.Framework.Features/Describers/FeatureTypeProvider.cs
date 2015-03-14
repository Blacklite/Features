using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Describers
{
    public class FeatureTypeProvider : IFeatureTypeProvider
    {
        public FeatureTypeProvider(IFeatureAssemblyProvider assemblyProvider, IFeatureDescriberFactory factory)
        {
            var featureTypeInfo = typeof(IFeature).GetTypeInfo();

            FeatureTypes = assemblyProvider.Assemblies
                .SelectMany(x => x.DefinedTypes)
                .Where(x => featureTypeInfo.IsAssignableFrom(x))
                .ToArray();
        }

        public IEnumerable<TypeInfo> FeatureTypes { get; }
    }
}
