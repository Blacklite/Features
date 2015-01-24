using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Blacklite.Framework.Features
{
    public interface IFeatureDescriberFactory
    {
        IEnumerable<IFeatureDescriber> Create(IEnumerable<IServiceDescriptor> descriptors);
    }

    class FeatureDescriberFactory : IFeatureDescriberFactory
    {
        public IEnumerable<IFeatureDescriber> Create(IEnumerable<IServiceDescriptor> descriptors)
        {
            return Fixup(descriptors.Select(x => new FeatureDescriber(x)));
        }

        private IEnumerable<FeatureDescriber> Fixup(IEnumerable<FeatureDescriber> describers)
        {
            foreach (var describer in describers)
            {
                describer.Children = describers.Where(x => x.Parent == describer.FeatureTypeInfo).ToArray();

                var requires = describers
                    .Join(describer.Requires, x => x.FeatureTypeInfo, x => x.FeatureType, (z, x) => z);

                var requiresDictionary = requires.Join(describer.Requires, x => x.FeatureTypeInfo,
                    x => x.FeatureType, (d, x) => new { d, x.IsEnabled }).ToDictionary(x => (IFeatureDescriber)x.d, x => x.IsEnabled);

                describer.DependsOn = new ReadOnlyDictionary<IFeatureDescriber, bool>(requiresDictionary);

                ValidateDescriber(describer);


                yield return describer;
            }
        }

        internal static void ValidateDescriber<T>(T describer)
            where T : FeatureDescriber
        {
            var requires = describer.DependsOn.Keys.Cast<T>();
            if (describer.Lifecycle == LifecycleKind.Singleton && requires.Any(z => z.Lifecycle == LifecycleKind.Scoped))
            {
                throw new NotSupportedException($"Lifecycle '{LifecycleKind.Scoped}' cannot be required by features with a lifecycle of '{describer.Lifecycle}'.");
            }

            if ((describer.Lifecycle == LifecycleKind.Singleton || describer.Lifecycle == LifecycleKind.Scoped) && requires.Any(z => z.Lifecycle == LifecycleKind.Transient))
            {
                throw new NotSupportedException($"Lifecycle '{LifecycleKind.Transient}' cannot be required by features with a lifecycle of '{describer.Lifecycle}'.");
            }

            if (describer.IsObservable && (describer.Lifecycle == LifecycleKind.Scoped || describer.Lifecycle == LifecycleKind.Transient))
            {
                throw new NotSupportedException($"Lifecycle '{describer.Lifecycle}' is not supported by observable features'.");
            }
        }
    }
}