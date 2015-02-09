using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.EditorModel
{
    public class FeatureEditor
    {
        private readonly IEnumerable<FeatureModel> _models;
        private readonly Func<Type, IFeature> _getFeature;
        private readonly Func<Type, object> _getFeatureOption;

        public FeatureEditor(IEnumerable<FeatureModel> models, Func<Type, IFeature> feature, Func<Type, object> featureOption)
        {
            _models = models;
            _getFeature = feature;
            _getFeatureOption = featureOption;
        }

        private JsonSchema _schema;
        public JsonSchema Schema
        {
            get
            {
                return _schema ?? (_schema = GenerateSchema());
            }
        }

        private JObject _model;
        public JObject Model
        {
            get
            {
                return _model ?? (_model = GenerateJObject());
            }
        }

        private JObject GenerateJObject()
        {
            var json = new JObject();
            foreach (var model in _models)
            {
                json.Add(model.Name, GetModelJObject(model));
                //model.Properties.Add(model.Name, GetModelSchema(model));
            }
            return json;
        }

        private JObject GetModelJObject(FeatureModel model)
        {
            var json = new JObject();

            if (model.Enabled.OptionsHasIsEnabled)
                json.Add("Enabled", new JValue(model.Enabled.GetValue(_getFeatureOption(model.FeatureType))));
            else
                json.Add("Enabled", new JValue(model.Enabled.GetValue(_getFeature(model.FeatureType))));

            if (model.Properties.Any())
            {
                var options = new JObject();
                json.Add("Options", options);

                foreach (var property in model.Properties)
                {
                    options.Add(property.Name, new JValue(property.GetValue(_getFeatureOption(model.FeatureType))));
                }
            }

            foreach (var child in model.Children)
            {
                json.Add(child.Name, GetModelJObject(child));
            }

            return json;
        }

        private JsonSchema GenerateSchema()
        {
            var schema = new JsonSchema();
            schema.Title = "Features";
            schema.Type = JsonSchemaType.Object;
            schema.Properties = new Dictionary<string, JsonSchema>();

            foreach (var model in _models)
            {
                schema.Properties.Add(model.Name, GetModelSchema(model));
            }

            return schema;
        }

        private JsonSchema GetModelSchema(FeatureModel model)
        {
            var schema = new JsonSchema();
            schema.Type = JsonSchemaType.Object;
            schema.Title = model.Name;
            schema.Description = model.Description;
            schema.Properties = new Dictionary<string, JsonSchema>();

            schema.Properties.Add("Enabled", GetPropertySchema(model.Enabled));

            if (model.Properties.Any())
            {
                var options = new JsonSchema();
                options.Properties = new Dictionary<string, JsonSchema>();
                options.Title = "Options";
                options.Type = JsonSchemaType.Object;
                foreach (var property in model.Properties.Skip(1))
                {
                    options.Properties.Add(property.Name, GetPropertySchema(property));
                }

                schema.Properties.Add("Options", options);
            }

            foreach (var child in model.Children)
            {
                schema.Properties.Add(child.Name, GetModelSchema(child));
            }

            return schema;
        }

        private JsonSchema GetPropertySchema(FeatureOptionPropertyModel property)
        {
            // change this
            var schema = new JsonSchemaGenerator().Generate(property.Type);
            schema.Description = property.Description;
            schema.ReadOnly = property.IsReadOnly;
            return schema;
        }
    }

    public class FeatureModel
    {
        public FeatureModel(IFeatureDescriber describer)
        {
            Name = describer.FeatureType.Name;
            Description = describer.Description;
            FeatureType = describer.FeatureType;
            Children = describer.Children.Select(x => new FeatureModel(x)).ToArray();
            Enabled = new FeatureOptionPropertyModel(typeof(bool), nameof(IFeature.IsEnabled), null, x => describer.GetIsEnabled<bool>(x), describer.IsReadOnly, describer.OptionsHasIsEnabled);
            Properties = GetProperties(describer).ToArray();
            Dependencies = describer.DependsOn.Select(x => new FeatureDependencyModel(x.Key, x.Value)).ToArray();
        }

        private IEnumerable<FeatureOptionPropertyModel> GetProperties(IFeatureDescriber describer)
        {
            if (describer.HasOptions)
            {
                var properties = describer.OptionsType.GetRuntimeProperties();

                foreach (var property in properties.Where(x => x.Name != nameof(IFeature.IsEnabled)))
                {
                    yield return new FeatureOptionPropertyModel(property.PropertyType, GetPropertyDisplayName(property), GetPropertyDescription(property), property.GetValue, property.CanWrite);
                }
            }
        }

        private string GetPropertyDisplayName(PropertyInfo property) => property.GetCustomAttribute<DisplayAttribute>()?.Name ?? property.Name;
        private string GetPropertyDescription(PropertyInfo property) => property.GetCustomAttribute<DisplayAttribute>()?.Description ?? property.Name;

        public string Name { get; }
        public Type FeatureType { get; }
        public string Description { get; }
        public FeatureOptionPropertyModel Enabled { get; }
        public IEnumerable<FeatureModel> Children { get; }
        public IEnumerable<FeatureDependencyModel> Dependencies { get; }
        public IEnumerable<FeatureOptionPropertyModel> Properties { get; }
    }

    public class FeatureDependencyModel
    {
        public FeatureDependencyModel(IFeatureDescriber describer, bool isEnabled)
        {
            Feature = describer;
            IsEnabled = isEnabled;
        }

        public IFeatureDescriber Feature { get; }
        public bool IsEnabled { get; }
    }

    public class FeatureOptionPropertyModel
    {
        public FeatureOptionPropertyModel(Type type, string name, string description, Func<object, object> getValue, bool isReadOnly = false, bool optionsHasIsEnabled = false)
        {
            Type = type;
            Name = name;
            Description = description;
            GetValue = getValue;
            IsReadOnly = isReadOnly;
            OptionsHasIsEnabled = optionsHasIsEnabled;
        }
        public Type Type { get; }
        public string Name { get; }
        public string Description { get; }
        public Func<object, object> GetValue { get; }
        public bool IsReadOnly { get; }
        public bool OptionsHasIsEnabled { get; }
    }
}