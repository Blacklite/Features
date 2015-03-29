using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Describers
{
    public class FeatureDescriberFactory : IFeatureDescriberFactory
    {
        public IEnumerable<IFeatureDescriber> Create(IEnumerable<TypeInfo> descriptors)
        {
            return Fixup(descriptors.Select(x => new FeatureDescriber(x)));
        }

        private IEnumerable<FeatureDescriber> Fixup(IEnumerable<FeatureDescriber> describers)
        {
            foreach (var describer in describers)
            {
                describer.Children = describers.Where(x => x.Parent == describer.TypeInfo).ToArray();

                var requires = describers
                    .Join(describer.Requires, x => x.TypeInfo, x => x.FeatureType, (z, x) => z);

                var requiresDictionary = requires.Join(describer.Requires, x => x.TypeInfo,
                    x => x.FeatureType, (d, x) => new { d, x.IsEnabled }).ToDictionary(x => (IFeatureDescriber)x.d, x => x.IsEnabled);

                describer.DependsOn = new ReadOnlyDictionary<IFeatureDescriber, bool>(requiresDictionary);

                ValidateDescriber(describer);


                yield return describer;
            }
        }

        public static void ValidateDescriber<T>(T describer)
            where T : FeatureDescriber
        {
            var requires = describer.DependsOn.Keys.Cast<T>();
            if (describer.IsObservable && requires.Any(z => !z.IsObservable))
            {
                throw new NotSupportedException($"Observable features cannot depend on non-observable features.");
            }
        }
    }
}
