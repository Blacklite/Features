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
        public RequiredFeatureAttribute(Type featureType, bool isEnabled = true)
        {
            FeatureType = featureType.GetTypeInfo();
            IsEnabled = isEnabled;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FeatureDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }
        public FeatureDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FeatureDescriptionAttribute : Attribute
    {
        public string Description { get; }
        public FeatureDescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class FeatureGroupAttribute : Attribute
    {
        public string[] Groups { get; }
        public FeatureGroupAttribute(params string[] groups)
        {
            Groups = groups;
        }
    }
}
