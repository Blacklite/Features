using System;
using System.Reflection;

namespace Blacklite.Framework.Features.Describers
{
    public class FeatureOptionsDescriber : IFeatureOptionsDescriber
    {
        public FeatureOptionsDescriber(Type optionsType)
        {
            Type = optionsType;
            TypeInfo = optionsType.GetTypeInfo();

            DisplayName = TypeInfo.GetCustomAttribute<FeatureDisplayNameAttribute>()?.DisplayName ?? Type.Name.AsUserFriendly();
            Description = TypeInfo.GetCustomAttribute<FeatureDescriptionAttribute>()?.Description;
            IsFeature = typeof(IFeature).GetTypeInfo().IsAssignableFrom(TypeInfo);
            IsSwitch = typeof(ISwitch).GetTypeInfo().IsAssignableFrom(TypeInfo);
        }

        public Type Type { get; }
        public TypeInfo TypeInfo { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public bool IsFeature { get; }
        public bool IsSwitch { get; }
    }
}
