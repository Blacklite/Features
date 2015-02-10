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
    }

    public class FeatureModel
    {
        public FeatureModel(IFeatureDescriber describer)
        {
            Name = describer.FeatureType.Name;
            Description = describer.Description;
            FeatureType = describer.FeatureType;
            OptionsType = describer.OptionsType;
            Children = describer.Children.Select(x => new FeatureModel(x)).ToArray();
            Enabled = new FeatureOptionPropertyModel(typeof(bool), nameof(IFeature.IsEnabled), null, x => describer.GetIsEnabled<bool>(x), describer.IsReadOnly, describer.OptionsHasIsEnabled);
            Properties = GetProperties(describer).ToDictionary(x => x.Name);
            Dependencies = describer.DependsOn.Select(x => new FeatureDependencyModel(x.Key, x.Value)).ToArray();
        }

        private IEnumerable<FeatureOptionPropertyModel> GetProperties(IFeatureDescriber describer)
        {
            if (describer.HasOptions)
            {
                var properties = describer.OptionsType.GetRuntimeProperties();

                foreach (var property in properties.Where(x => x.Name != nameof(IFeature.IsEnabled)))
                {
                    yield return new FeatureOptionPropertyModel(property.PropertyType, property.Name, GetPropertyDescription(property), property.GetValue, !property.CanWrite);
                }
            }
        }

        private string GetPropertyDescription(PropertyInfo property) => property.GetCustomAttribute<DisplayAttribute>()?.Description;

        public string Name { get; }
        public Type FeatureType { get; }
        public Type OptionsType { get; }
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
