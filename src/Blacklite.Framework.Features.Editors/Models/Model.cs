using Blacklite.Framework.Features.Describers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Editors.Models
{
    public class Model : GroupOrModel
    {
        public Model(IFeatureDescriber describer) : base(describer.Type.Name)
        {
            Describer = describer;
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
            Children = describer.Children.Select(x => new Model(x)).ToArray();
            if (HasEnabled)
                Enabled = new OptionPropertyModel(typeof(bool),
                    nameof(ISwitch.IsEnabled),
                    "IsEnabled",
                    null,
                    x => describer.GetIsEnabled<bool>(x),
                    (obj, value) => describer.SetIsEnabled(obj, value),
                    describer.IsReadOnly);
            Options = GetOptions(describer).ToDictionary(x => x.Name);
            Properties = GetProperties(describer).ToDictionary(x => x.Name);
            Dependencies = describer.DependsOn.Select(x => new DependencyModel(x.Key, x.Value)).ToArray();
        }

        private IEnumerable<OptionPropertyModel> GetOptions(IFeatureDescriber describer)
        {
            if (describer.HasOptions && !describer.Options.IsFeature)
            {
                var properties = describer.Options.Type.GetRuntimeProperties();

                foreach (var property in properties.Where(x => x.Name != nameof(ISwitch.IsEnabled)))
                {
                    yield return new OptionPropertyModel(property.PropertyType, property.Name, GetPropertyDisplayName(property), GetPropertyDescription(property), property.GetValue, property.SetValue, !property.CanWrite);
                }
            }
        }

        private IEnumerable<OptionPropertyModel> GetProperties(IFeatureDescriber describer)
        {
            foreach (var property in describer.Properties)
            {
                yield return new OptionPropertyModel(property.Type, property.Name, property.DisplayName, property.Description, (x) => property.GetProperty<object>(x), property.SetProperty, property.IsReadOnly);
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
        public OptionPropertyModel Enabled { get; }
        public IDictionary<string, OptionPropertyModel> Options { get; }
        public IDictionary<string, OptionPropertyModel> Properties { get; }
        public IEnumerable<Model> Children { get; }
        public IEnumerable<DependencyModel> Dependencies { get; }
        public Model OptionsFeature { get; set; }
        public IFeatureDescriber Describer { get; }


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
            var typed = obj as Model;
            if (typed != null)
                return typed.Name.Equals(this.Name);

            return false;
        }
    }
}
