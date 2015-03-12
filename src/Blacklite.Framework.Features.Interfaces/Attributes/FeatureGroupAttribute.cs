using System;

namespace Blacklite.Framework.Features
{
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
