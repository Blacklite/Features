using System;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ParentFeatureAttribute : Attribute
    {
        public TypeInfo Feature { get; }
        public ParentFeatureAttribute(Type featureType)
        {
            Feature = featureType.GetTypeInfo();
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequiredFeatureAttribute : Attribute
    {
        public TypeInfo FeatureType { get; }
        public bool IsEnabled { get; }
        public RequiredFeatureAttribute(Type featureType, bool isEnabled)
        {
            FeatureType = featureType.GetTypeInfo();
            IsEnabled = isEnabled;
        }
    }
}
