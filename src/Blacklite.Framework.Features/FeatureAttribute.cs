using System;

namespace Blacklite.Framework.Features
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ParentFeatureAttribute : Attribute
    {
        public Type Feature { get; }
        public ParentFeatureAttribute(Type featureType)
        {
            Feature = featureType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequiredFeatureAttribute : Attribute
    {
        public Type FeatureType { get; }
        public bool IsEnabled { get; }
        public RequiredFeatureAttribute(Type featureType, bool isEnabled)
        {
            FeatureType = featureType;
            IsEnabled = isEnabled;
        }
    }
}
