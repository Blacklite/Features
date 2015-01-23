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
        bool IsScoped { get; }
        bool IsObservable { get; }
        IReadOnlyDictionary<IFeatureDescriber, bool> DependsOn { get; }
        IEnumerable<IFeatureDescriber> Children { get; }
    }

    class FeatureDescriber : IFeatureDescriber
    {
        public FeatureDescriber(TypeInfo type)
        {
            FeatureType = type.AsType();
            IsScoped = type.ImplementedInterfaces.Contains(typeof(IScopedFeature));
            IsObservable = type.ImplementedInterfaces.Contains(typeof(IObservableFeature));

            Requires = type.GetCustomAttributes<RequiredFeatureAttribute>();
            Parent = type.GetCustomAttribute<ParentFeatureAttribute>()?.Feature.GetTypeInfo();

            DependsOn = new ReadOnlyDictionary<IFeatureDescriber, bool>(new Dictionary<IFeatureDescriber, bool>());
            Children = Enumerable.Empty<IFeatureDescriber>();
        }

        public Type FeatureType { get; }
        public bool IsScoped { get; }
        public bool IsObservable { get; }

        private IEnumerable<RequiredFeatureAttribute> Requires { get; }
        private TypeInfo Parent { get; }

        public IReadOnlyDictionary<IFeatureDescriber, bool> DependsOn { get; private set; }
        public IEnumerable<IFeatureDescriber> Children { get; private set; }

        public static FeatureDescriber Create(TypeInfo type)
        {
            return new FeatureDescriber(type);
        }

        public static IEnumerable<FeatureDescriber> Fixup(IEnumerable<FeatureDescriber> describers)
        {
            foreach (var describer in describers)
            {
                describer.Children = describers.Where(x => x.Parent == describer.FeatureType.GetTypeInfo()).ToArray();
                describer.DependsOn = new ReadOnlyDictionary<IFeatureDescriber, bool>(
                    describer.Requires
                        .ToDictionary(
                            x => (IFeatureDescriber)describers.Single(z => z.FeatureType == x.FeatureType),
                            x => x.IsEnabled)
                        );

                yield return describer;
            }
        }
    }
}
