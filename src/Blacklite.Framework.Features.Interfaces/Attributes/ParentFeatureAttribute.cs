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
}
