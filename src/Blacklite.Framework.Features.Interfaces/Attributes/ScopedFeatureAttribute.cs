using Microsoft.Framework.DependencyInjection;
using System;

namespace Blacklite.Framework.Features
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ScopedFeatureAttribute : Attribute
    {
    }
}
