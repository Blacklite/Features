using Blacklite.Framework.Features.Describers;
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
        public FeatureModel(IFeatureDescriber describer) : base(describer.Type.Name)
        {
            Title = describer.DisplayName;
            Description = describer.Description;
            FeatureType = describer.Type;
            OptionsType = describer?.Options?.Type;
            OptionsTitle = describer?.Options?.DisplayName;
            OptionsName = describer?.Options?.Type?.Name?.CamelCase();
            HasEnabled = describer.HasEnabled;
            HasOptions = describer.HasOptions;
            HasProperties = describer.Properties.Any();
            OptionsIsFeature = describer?.Options?.IsFeature ?? false;
            OptionsDescription = describer?.Options?.Description;
            Children = describer.Children.Select(x => new FeatureModel(x)).ToArray();
            if (HasEnabled)
                Enabled = new FeatureOptionPropertyModel(typeof(bool),
                    nameof(ISwitch.IsEnabled),
                    "Enabled",
                    null,
                    x => describer.GetIsEnabled<bool>(x),
                    (obj, value) => describer.SetIsEnabled(obj, value),
                    describer.IsReadOnly);
            Options = GetOptions(describer).ToDictionary(x => x.Name);
            Properties = GetProperties(describer).ToDictionary(x => x.Name);
            Dependencies = describer.DependsOn.Select(x => new FeatureDependencyModel(x.Key, x.Value)).ToArray();
        }

        private IEnumerable<FeatureOptionPropertyModel> GetOptions(IFeatureDescriber describer)
        {
            if (describer.HasOptions && !describer.Options.IsFeature)
            {
                var properties = describer.Options.Type.GetRuntimeProperties();

                foreach (var property in properties.Where(x => x.Name != nameof(ISwitch.IsEnabled)))
                {
                    yield return new FeatureOptionPropertyModel(property.PropertyType, property.Name, GetPropertyDisplayName(property), GetPropertyDescription(property), property.GetValue, property.SetValue, !property.CanWrite);
                }
            }
        }

        private IEnumerable<FeatureOptionPropertyModel> GetProperties(IFeatureDescriber describer)
        {
            foreach (var property in describer.Properties)
            {
                yield return new FeatureOptionPropertyModel(property.Type, property.Name, property.DisplayName, property.Description, (x) => property.GetProperty<object>(x), property.SetProperty, property.IsReadOnly);
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
        public bool HasOptions { get; }
        public bool HasProperties { get; }
        public bool OptionsIsFeature { get; }
        public string Description { get; }
        public FeatureOptionPropertyModel Enabled { get; }
        public IDictionary<string, FeatureOptionPropertyModel> Options { get; }
        public IDictionary<string, FeatureOptionPropertyModel> Properties { get; }
        public IEnumerable<FeatureModel> Children { get; }
        public IEnumerable<FeatureDependencyModel> Dependencies { get; }
        public FeatureModel OptionsFeature { get; set; }


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
