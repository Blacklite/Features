using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.EditorModel
{
    public class FeatureModelGroups
    {
        public IGrouping<string, FeatureModel> Groups { get; }
        public IEnumerable<FeatureModel> Models { get; }
    }

    public abstract class FeatureGroupOrModel
    {
        public FeatureGroupOrModel(string name)
        {
            Name = name.CamelCase();
        }
        public string Name { get; }
    }

    public class FeatureGroup : FeatureGroupOrModel
    {
        public FeatureGroup(string name) : base(name)
        {
            Title = name;
            Items = new List<FeatureGroupOrModel>();
        }

        public string Title { get; }
        public IList<FeatureGroupOrModel> Items { get; }
    }

    public class FeatureModel : FeatureGroupOrModel
    {
        public FeatureModel(IFeatureDescriber describer) : base(describer.FeatureType.Name)
        {
            Title = describer.DisplayName;
            Description = describer.Description;
            FeatureType = describer.FeatureType;
            OptionsType = describer.OptionsType;
            OptionsTitle = describer.OptionsDisplayName;
            OptionsName = describer.OptionsType?.Name?.CamelCase();
            HasEnabled = describer.HasEnabled;
            OptionsHasIsEnabled = describer.OptionsHasIsEnabled;
            OptionsDescription = describer.OptionsDescription;
            Children = describer.Children.Select(x => new FeatureModel(x)).ToArray();
            if (HasEnabled)
                Enabled = new FeatureOptionPropertyModel(typeof(bool),
                    nameof(ISwitch.IsEnabled),
                    "Enabled",
                    null,
                    x => describer.GetIsEnabled<bool>(x),
                    (obj, value) => describer.SetIsEnabled(obj, value),
                    describer.IsReadOnly, describer.OptionsHasIsEnabled);
            Properties = GetProperties(describer).ToDictionary(x => x.Name);
            Dependencies = describer.DependsOn.Select(x => new FeatureDependencyModel(x.Key, x.Value)).ToArray();
        }

        private IEnumerable<FeatureOptionPropertyModel> GetProperties(IFeatureDescriber describer)
        {
            if (describer.HasOptions)
            {
                var properties = describer.OptionsType.GetRuntimeProperties();

                foreach (var property in properties.Where(x => x.Name != nameof(ISwitch.IsEnabled)))
                {
                    yield return new FeatureOptionPropertyModel(property.PropertyType, property.Name, GetPropertyDisplayName(property), GetPropertyDescription(property), property.GetValue, property.SetValue, !property.CanWrite);
                }
            }
        }

        private string GetPropertyDescription(PropertyInfo property) => property.GetCustomAttribute<DisplayAttribute>()?.Description;
        private string GetPropertyDisplayName(PropertyInfo property) => property.GetCustomAttribute<DisplayAttribute>()?.Name ?? property.Name.AsUserFriendly();

        public string Title { get; }
        public Type FeatureType { get; }
        public Type OptionsType { get; }
        public string OptionsName { get; }
        public string OptionsTitle { get; }
        public string OptionsDescription { get; }
        public bool HasEnabled { get; }
        public bool OptionsHasIsEnabled { get; }
        public string Description { get; }
        public FeatureOptionPropertyModel Enabled { get; }
        public IDictionary<string, FeatureOptionPropertyModel> Properties { get; }
        public IEnumerable<FeatureModel> Children { get; }
        public IEnumerable<FeatureDependencyModel> Dependencies { get; }


        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return $"Editor for {this.Name}";
        }

        public override bool Equals(object obj)
        {
            var typed = obj as FeatureModel;
            if (typed != null)
                return typed.Name.Equals(this.Name);

            return false;
        }
    }
}
