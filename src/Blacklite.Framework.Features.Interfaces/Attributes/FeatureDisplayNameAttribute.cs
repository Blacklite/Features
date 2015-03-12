using System;

namespace Blacklite.Framework.Features
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FeatureDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }
        public FeatureDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
