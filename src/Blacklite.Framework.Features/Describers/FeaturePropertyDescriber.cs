using System;
using System.Reflection;

namespace Blacklite.Framework.Features.Describers
{
    public class FeaturePropertyDescriber : IFeaturePropertyDescriber
    {
        private readonly PropertyInfo _propertyInfo;
        public FeaturePropertyDescriber(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
            Name = propertyInfo.Name;
            Type = propertyInfo.PropertyType;
            TypeInfo = propertyInfo.PropertyType.GetTypeInfo();

            DisplayName = TypeInfo.GetCustomAttribute<FeatureDisplayNameAttribute>()?.DisplayName ?? propertyInfo.Name.AsUserFriendly();
            Description = TypeInfo.GetCustomAttribute<FeatureDescriptionAttribute>()?.Description;

            IsReadOnly = !_propertyInfo.CanWrite;
        }

        public string Description { get; }
        public string DisplayName { get; }
        public Type Type { get; }
        public TypeInfo TypeInfo { get; }
        public bool IsReadOnly { get; }
        public string Name { get; }

        public void SetProperty(object instance, object value)
        {
            if (!IsReadOnly)
                _propertyInfo.SetValue(instance, value);
        }

        public T GetProperty<T>(object instance)
        {
            return (T)_propertyInfo.GetValue(instance);
        }
    }
}
