using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    public interface IFeatureDescriber
    {
        Type FeatureType { get; }
        LifecycleKind Lifecycle { get; }
        bool IsObservable { get; }
        IReadOnlyDictionary<IFeatureDescriber, bool> DependsOn { get; }
        IEnumerable<IFeatureDescriber> Children { get; }
    }

    class FeatureDescriber : IFeatureDescriber
    {
        public FeatureDescriber(IServiceDescriptor descriptor, LifecycleKind lifecycle)
        {
            FeatureType = descriptor.ServiceType;
            FeatureTypeInfo = FeatureType.GetTypeInfo();
            Lifecycle = descriptor.Lifecycle;
            IsObservable = FeatureTypeInfo.ImplementedInterfaces.Contains(typeof(IObservableFeature));

            Requires = FeatureTypeInfo.GetCustomAttributes<RequiredFeatureAttribute>();
            Parent = FeatureTypeInfo.GetCustomAttribute<ParentFeatureAttribute>()?.Feature;

            DependsOn = new ReadOnlyDictionary<IFeatureDescriber, bool>(new Dictionary<IFeatureDescriber, bool>());
            Children = Enumerable.Empty<IFeatureDescriber>();
        }

        public Type FeatureType { get; }
        public TypeInfo FeatureTypeInfo { get; }
        public LifecycleKind Lifecycle { get; }
        public bool IsObservable { get; }

        private IEnumerable<RequiredFeatureAttribute> Requires { get; }
        private TypeInfo Parent { get; }

        public IReadOnlyDictionary<IFeatureDescriber, bool> DependsOn { get; private set; }
        public IEnumerable<IFeatureDescriber> Children { get; private set; }

        public static FeatureDescriber Create(IServiceDescriptor descriptor)
        {
            return new FeatureDescriber(descriptor, descriptor.Lifecycle);
        }

        public static IEnumerable<FeatureDescriber> Fixup(IEnumerable<FeatureDescriber> describers)
        {
            foreach (var describer in describers)
            {
                describer.Children = describers.Where(x => x.Parent == describer.FeatureTypeInfo).ToArray();

                var requires = describers
                    .Join(describer.Requires, x => x.FeatureTypeInfo, x => x.FeatureType, (z, x) => z);

                if (describer.Lifecycle == LifecycleKind.Singleton && requires.Any(z => z.Lifecycle == LifecycleKind.Scoped))
                {
                    throw new NotSupportedException($"Lifecycle '{LifecycleKind.Scoped}' is not supported for features with a lifecycle of '{describer.Lifecycle}'.");
                }

                if ((describer.Lifecycle == LifecycleKind.Singleton || describer.Lifecycle == LifecycleKind.Scoped) && requires.Any(z => z.Lifecycle == LifecycleKind.Transient))
                {
                    throw new NotSupportedException($"Lifecycle '{LifecycleKind.Transient}' is not supported for features with a lifecycle of '{describer.Lifecycle}'.");
                }

                if (describer.IsObservable && (describer.Lifecycle == LifecycleKind.Scoped || describer.Lifecycle == LifecycleKind.Transient))
                {
                    throw new NotSupportedException($"Lifecycle '{describer.Lifecycle}' is not supported by observable features'.");
                }

                var requiresDictionary = requires.Join(describer.Requires, x => x.FeatureTypeInfo,
                    x => x.FeatureType, (d, x) => new { d, x.IsEnabled }).ToDictionary(x => (IFeatureDescriber)x.d, x => x.IsEnabled);

                describer.DependsOn = new ReadOnlyDictionary<IFeatureDescriber, bool>(requiresDictionary);

                yield return describer;
            }
        }
    }
}
