using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
#if ASPNET50 || ASPNETCORE50
using Microsoft.Framework.Runtime;
#endif

namespace Blacklite.Framework.Features.Describers
{
    class FeatureDescriberProvider : IFeatureDescriberProvider
    {
        public FeatureDescriberProvider(
            IFeatureTypeProvider featureTypeProvider,
            IFeatureDescriberFactory factory)
        {
            var featureTypeInfo = typeof(IFeature).GetTypeInfo();

            var dictionary = factory.Create(featureTypeProvider.FeatureTypes)
                .ToDictionary(x => x.Type);

            Describers = new ReadOnlyDictionary<Type, IFeatureDescriber>(dictionary);
        }

        public IReadOnlyDictionary<Type, IFeatureDescriber> Describers { get; }
    }
}
