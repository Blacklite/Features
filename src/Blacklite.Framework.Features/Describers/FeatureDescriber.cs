using Blacklite.Framework.Features.OptionsModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Describers
{
    public class FeatureDescriber : IFeatureDescriber
    {
        private readonly PropertyInfo _isEnabledProperty;
        private readonly PropertyInfo _optionsProperty;

        public FeatureDescriber(TypeInfo typeInfo)
        {
            Type = typeInfo.AsType();
            TypeInfo = typeInfo;
            IsObservable = TypeInfo.ImplementedInterfaces.Contains(typeof(IObservableFeature));
            HasOptions = TypeInfo.ImplementedInterfaces.Contains(typeof(IFeatureOptions));

            if (HasOptions)
            {
                _optionsProperty = Type
                    .GetRuntimeProperty(nameof(ISwitch<object>.Options));

                Options = new FeatureOptionsDescriber(_optionsProperty.PropertyType);
            }
            HasEnabled = typeof(ISwitch).GetTypeInfo().IsAssignableFrom(TypeInfo);

            if (HasEnabled)
            {
                var isEnabledProperty = Type.GetRuntimeProperty(nameof(ISwitch.IsEnabled));
                // If we are not observable, and our Lifetime is a singleton, changes in our value cannot accurately be observed.
                IsReadOnly = !isEnabledProperty.CanWrite;// || (!IsObservable && Lifetime == ServiceLifetime.Singleton);
                _isEnabledProperty = isEnabledProperty;
            }

            var properties = Type.GetRuntimeProperties();

            if (HasEnabled)
                properties = properties.Where(x => x.Name != nameof(ISwitch.IsEnabled));
            if (HasOptions)
                properties = properties.Where(x => x.Name != nameof(ISwitch<object>.Options));

            Properties = properties.Select(x => new FeaturePropertyDescriber(x));

            Requires = TypeInfo.GetCustomAttributes<RequiredFeatureAttribute>();
            Parent = TypeInfo.GetCustomAttribute<ParentFeatureAttribute>()?.Feature;

            DependsOn = new ReadOnlyDictionary<IFeatureDescriber, bool>(new Dictionary<IFeatureDescriber, bool>());
            Children = Enumerable.Empty<IFeatureDescriber>();

            DisplayName = TypeInfo.GetCustomAttribute<FeatureDisplayNameAttribute>()?.DisplayName ?? Type.Name.AsUserFriendly();
            Description = TypeInfo.GetCustomAttribute<FeatureDescriptionAttribute>()?.Description;

            Groups = TypeInfo.GetCustomAttributes<FeatureGroupAttribute>()?.SelectMany(x => x.Groups).ToArray();
            if (Parent == null && !Groups.Any())
            {
                Groups = new[] { string.Empty };
            }
        }

        public Type Type { get; }
        public TypeInfo TypeInfo { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public bool IsObservable { get; }
        public bool HasOptions { get; }
        public bool HasEnabled { get; }
        public bool IsReadOnly { get; }
        public IFeatureOptionsDescriber Options { get; }

        public IEnumerable<RequiredFeatureAttribute> Requires { get; }
        public TypeInfo Parent { get; }

        public IReadOnlyDictionary<IFeatureDescriber, bool> DependsOn { get; set; }
        public IEnumerable<IFeatureDescriber> Children { get; set; }

        public IEnumerable<string> Groups { get; }

        public IEnumerable<IFeaturePropertyDescriber> Properties { get; }

        public T GetIsEnabled<T>(object instance)
        {
            if (!HasEnabled)
                return default(T);
            return (T)_isEnabledProperty.GetValue(instance);
        }

        public void SetIsEnabled<T>(object instance, T value)
        {
            if (HasEnabled && !IsReadOnly)
                _isEnabledProperty.SetValue(instance, value);
        }

        public T GetOptions<T>(object instance)
        {
            if (HasOptions)
                return (T)_optionsProperty.GetValue(instance);
            return default(T);
        }

        public override int GetHashCode()
        {
            return TypeInfo.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var item = obj as FeatureDescriber;
            if (item != null)
                return TypeInfo.Equals(item.TypeInfo);
            return false;
        }
    }
}
