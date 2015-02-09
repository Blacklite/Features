using Blacklite.Framework.Features.OptionModel;
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
        Type OptionsType { get; }
        TypeInfo OptionsTypeInfo { get; }
        LifecycleKind Lifecycle { get; }
        bool IsObservable { get; }
        bool HasOptions { get; }
        bool IsReadOnly { get; }
        bool OptionsHasIsEnabled { get; }
        string DisplayName { get; }
        string Description { get; }
        IReadOnlyDictionary<IFeatureDescriber, bool> DependsOn { get; }
        IEnumerable<IFeatureDescriber> Children { get; }

        IEnumerable<RequiredFeatureAttribute> Requires { get; }
        TypeInfo Parent { get; }

        T GetIsEnabled<T>(object instance);
        void SetIsEnabled<T>(object instance, T value);
        T GetOptions<T>(object instance);
    }

    class FeatureDescriber : IFeatureDescriber
    {
        private readonly PropertyInfo _isEnabledProperty;
        private readonly PropertyInfo _optionsProperty;
        public FeatureDescriber(IServiceDescriptor descriptor)
        {
            FeatureType = descriptor.ServiceType;
            FeatureTypeInfo = FeatureType.GetTypeInfo();
            Lifecycle = descriptor.Lifecycle;
            IsObservable = FeatureTypeInfo.ImplementedInterfaces.Contains(typeof(IObservableFeature));

            HasOptions = FeatureTypeInfo.ImplementedInterfaces.Contains(typeof(IFeatureOptions));

            var isEnabledProperty = FeatureTypeInfo.FindDeclaredProperty(nameof(IFeature.IsEnabled));
            if (HasOptions)
            {
                _optionsProperty = FeatureTypeInfo
                    .FindDeclaredProperty(nameof(IFeature<object>.Options));

                OptionsType = _optionsProperty.PropertyType;
                OptionsTypeInfo = _optionsProperty.PropertyType.GetTypeInfo();

                var property = _optionsProperty?.PropertyType?.GetTypeInfo()
                    ?.FindDeclaredProperty(nameof(IFeature.IsEnabled));

                if (property != null)
                {
                    isEnabledProperty = property;
                    OptionsHasIsEnabled = true;
                }
            }
            IsReadOnly = !isEnabledProperty.CanWrite;

            _isEnabledProperty = isEnabledProperty;


            Requires = FeatureTypeInfo.GetCustomAttributes<RequiredFeatureAttribute>();
            Parent = FeatureTypeInfo.GetCustomAttribute<ParentFeatureAttribute>()?.Feature;

            DependsOn = new ReadOnlyDictionary<IFeatureDescriber, bool>(new Dictionary<IFeatureDescriber, bool>());
            Children = Enumerable.Empty<IFeatureDescriber>();

            DisplayName = FeatureTypeInfo.GetCustomAttribute<FeatureDisplayNameAttribute>()?.DisplayName ?? FeatureType.Name;
            Description = FeatureTypeInfo.GetCustomAttribute<FeatureDescriptionAttribute>()?.Description;
        }

        public Type FeatureType { get; }
        public TypeInfo FeatureTypeInfo { get; }
        public Type OptionsType { get; }
        public TypeInfo OptionsTypeInfo { get; }
        public LifecycleKind Lifecycle { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public bool IsObservable { get; }
        public bool HasOptions { get; }
        public bool OptionsHasIsEnabled { get; }
        public bool IsReadOnly { get; }

        public IEnumerable<RequiredFeatureAttribute> Requires { get; }
        public TypeInfo Parent { get; }

        public IReadOnlyDictionary<IFeatureDescriber, bool> DependsOn { get; internal set; }
        public IEnumerable<IFeatureDescriber> Children { get; internal set; }

        public T GetIsEnabled<T>(object instance)
        {
            return (T)_isEnabledProperty.GetValue(instance);
        }

        public void SetIsEnabled<T>(object instance, T value)
        {
            if (!IsReadOnly)
                _isEnabledProperty.SetValue(instance, value);
        }

        public T GetOptions<T>(object instance)
        {
            if (HasOptions)
                return (T)_optionsProperty.GetValue(instance);
            return default(T);
        }
    }
}
