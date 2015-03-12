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
        bool HasEnabled { get; }
        bool HasOptions { get; }
        bool IsReadOnly { get; }
        bool OptionsHasIsEnabled { get; }
        string DisplayName { get; }
        string Description { get; }
        string OptionsDisplayName { get; }
        string OptionsDescription { get; }
        IReadOnlyDictionary<IFeatureDescriber, bool> DependsOn { get; }
        IEnumerable<IFeatureDescriber> Children { get; }
        IEnumerable<string> Groups { get; }

        IEnumerable<RequiredFeatureAttribute> Requires { get; }
        TypeInfo Parent { get; }

        T GetIsEnabled<T>(object instance);
        void SetIsEnabled<T>(object instance, T value);
        T GetOptions<T>(object instance);
    }
}
