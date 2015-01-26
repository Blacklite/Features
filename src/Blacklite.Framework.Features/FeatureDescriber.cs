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
        TypeInfo FeatureTypeInfo { get; }
        LifecycleKind Lifecycle { get; }
        bool IsObservable { get; }

        IReadOnlyDictionary<IFeatureDescriber, bool> DependsOn { get; }
        IEnumerable<IFeatureDescriber> Children { get; }

        IEnumerable<RequiredFeatureAttribute> Requires { get; }
        TypeInfo Parent { get; }
    }

    class FeatureDescriber : IFeatureDescriber
    {
        public FeatureDescriber(IServiceDescriptor descriptor)
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

        public IEnumerable<RequiredFeatureAttribute> Requires { get; }
        public TypeInfo Parent { get; }

        public IReadOnlyDictionary<IFeatureDescriber, bool> DependsOn { get; internal set; }
        public IEnumerable<IFeatureDescriber> Children { get; internal set; }
    }
}
