using Blacklite.Framework.Features.OptionsModel;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    public class FeatureDescriber : IFeatureDescriber
    {
        private readonly PropertyInfo _isEnabledProperty;
        private readonly PropertyInfo _optionsProperty;
        private readonly IServiceDescriptor _descriptor;
        public FeatureDescriber(IServiceDescriptor descriptor)
        {
            _descriptor = descriptor;
            FeatureType = descriptor.ServiceType;
            FeatureTypeInfo = FeatureType.GetTypeInfo();
            Lifecycle = descriptor.Lifecycle;
            IsObservable = FeatureTypeInfo.ImplementedInterfaces.Contains(typeof(IObservableFeature));

            HasOptions = FeatureTypeInfo.ImplementedInterfaces.Contains(typeof(IFeatureOptions));

            var isEnabledProperty = FeatureTypeInfo.FindDeclaredProperty(nameof(ISwitch.IsEnabled));
            if (HasOptions)
            {
                _optionsProperty = FeatureTypeInfo
                    .FindDeclaredProperty(nameof(ISwitch<object>.Options));

                OptionsType = _optionsProperty.PropertyType;
                OptionsTypeInfo = _optionsProperty.PropertyType.GetTypeInfo();

                var property = _optionsProperty?.PropertyType?.GetTypeInfo()
                    ?.FindDeclaredProperty(nameof(ISwitch.IsEnabled));

                if (property != null)
                {
                    isEnabledProperty = property;
                    OptionsHasIsEnabled = true;
                }

                OptionsDisplayName = OptionsTypeInfo.GetCustomAttribute<FeatureDisplayNameAttribute>()?.DisplayName ?? OptionsType.Name.AsUserFriendly();
                OptionsDescription = OptionsTypeInfo.GetCustomAttribute<FeatureDescriptionAttribute>()?.Description;
            }
            HasEnabled = typeof(ISwitch).GetTypeInfo().IsAssignableFrom(FeatureTypeInfo);

            if (HasEnabled)
            {
                // If we are not observable, and our lifecycle is a singleton, changes in our value cannot accurately be observed.
                IsReadOnly = !isEnabledProperty.CanWrite;// || (!IsObservable && Lifecycle == LifecycleKind.Singleton);

                _isEnabledProperty = isEnabledProperty;
            }

            Requires = FeatureTypeInfo.GetCustomAttributes<RequiredFeatureAttribute>();
            Parent = FeatureTypeInfo.GetCustomAttribute<ParentFeatureAttribute>()?.Feature;

            DependsOn = new ReadOnlyDictionary<IFeatureDescriber, bool>(new Dictionary<IFeatureDescriber, bool>());
            Children = Enumerable.Empty<IFeatureDescriber>();

            DisplayName = FeatureTypeInfo.GetCustomAttribute<FeatureDisplayNameAttribute>()?.DisplayName ?? FeatureType.Name.AsUserFriendly();
            Description = FeatureTypeInfo.GetCustomAttribute<FeatureDescriptionAttribute>()?.Description;

            Groups = FeatureTypeInfo.GetCustomAttributes<FeatureGroupAttribute>()?.SelectMany(x => x.Groups).ToArray();
            if (Parent == null && !Groups.Any())
            {
                Groups = new[] { "( not grouped )" };
            }

            GenericFeatureType = typeof(Feature<>).MakeGenericType(FeatureType);
            if (IsObservable)
            GenericObservableFeatureType = typeof(ObservableFeature<>).MakeGenericType(FeatureType);
        }

        public Type FeatureType { get; }
        public TypeInfo FeatureTypeInfo { get; }
        public Type OptionsType { get; }
        public TypeInfo OptionsTypeInfo { get; }
        public LifecycleKind Lifecycle { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string OptionsDisplayName { get; }
        public string OptionsDescription { get; }
        public bool IsObservable { get; }
        public bool HasOptions { get; }
        public bool HasEnabled { get; }
        public bool OptionsHasIsEnabled { get; }
        public bool IsReadOnly { get; }

        public IEnumerable<RequiredFeatureAttribute> Requires { get; }
        public TypeInfo Parent { get; }

        public IReadOnlyDictionary<IFeatureDescriber, bool> DependsOn { get; set; }
        public IEnumerable<IFeatureDescriber> Children { get; set; }

        public IEnumerable<string> Groups { get; }

        public Type GenericFeatureType { get; }

        public Type GenericObservableFeatureType { get; }

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
            return _descriptor.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var item = obj as FeatureDescriber;
            if (item != null)
                return _descriptor.Equals(item._descriptor);
            return false;
        }
    }
}
