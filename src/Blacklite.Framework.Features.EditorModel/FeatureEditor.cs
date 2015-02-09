using Temp.Newtonsoft.Json;
using Temp.Newtonsoft.Json.Converters;
using Temp.Newtonsoft.Json.Linq;
using Temp.Newtonsoft.Json.Schema;
using Temp.Newtonsoft.Json.Schema.Generation;
using Temp.Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.EditorModel
{
    class SchemaContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property =  base.CreateProperty(member, memberSerialization);
            return property;
        }
    }

    public class FeatureEditor
    {
        private readonly IEnumerable<FeatureModel> _models;
        private readonly Func<Type, IFeature> _getFeature;
        private readonly Func<Type, object> _getFeatureOption;
        private readonly JSchemaGenerator _schemaGenerator;
        private readonly JsonSerializer _serializer;

        public FeatureEditor(IEnumerable<FeatureModel> models, Func<Type, IFeature> feature, Func<Type, object> featureOption)
        {
            _models = models;
            _getFeature = feature;
            _getFeatureOption = featureOption;

            var contractResolver = new SchemaContractResolver();
            _schemaGenerator = new JSchemaGenerator()
            {
                ContractResolver = contractResolver,
            };
            _schemaGenerator.GenerationProviders.Add(new StringEnumGenerationProvider());

            _serializer = new JsonSerializer()
            {
                ContractResolver = contractResolver,
            };

            _serializer.Converters.Add(new StringEnumConverter());
        }

        private JSchema _schema;
        public JSchema Schema
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

            if (model.OptionsType != null)
            {
                json.Add("Options", JObject.FromObject(_getFeatureOption(model.FeatureType), _serializer));
            }

            foreach (var child in model.Children)
            {
                json.Add(child.Name, GetModelJObject(child));
            }

            return json;
        }

        private JSchema GenerateSchema()
        {
            var schema = new JSchema();
            schema.Title = "Features";

            schema.Type = JSchemaType.Object;
            //schema.Format = "tabs";
            foreach (var model in _models)
            {
                schema.Properties.Add(model.Name, GetModelSchema(model));
            }

            //schema.Type = JSchemaType.Array;
            //schema.Format = "tabs";
            //foreach (var model in _models)
            //{
            //    schema.Items.Add(GetModelSchema(model));
            //}

            return schema;
        }

        private JSchema GetModelSchema(FeatureModel model)
        {
            var schema = new JSchema();
            schema.Type = JSchemaType.Object;
            schema.Title = model.Name;
            schema.Description = model.Description;
            schema.ExtensionData["options"] = JObject.FromObject(new { disable_collapse = true });

            schema.Properties.Add("Enabled", GetOrUpdatePropertySchema(model.Enabled));

            if (model.OptionsType != null)
            {
                var options = _schemaGenerator.Generate(model.OptionsType);
                options.Title = "Options";
                options.ExtensionData["options"] = JObject.FromObject(new { collapsed = true });
                foreach (var property in options.Properties)
                {
                    GetOrUpdatePropertySchema(model.Properties[property.Key], property.Value);
                }
                schema.Properties.Add("Options", options);
            }

            foreach (var child in model.Children)
            {
                schema.Properties.Add(child.Name, GetModelSchema(child));
            }

            return schema;
        }

        private JSchema GetOrUpdatePropertySchema(FeatureOptionPropertyModel property, JSchema schema = null)
        {
            // change this
            schema = schema ?? _schemaGenerator.Generate(property.Type);
            schema.Description = property.Description;

            schema.ExtensionData["readonly"] = new JValue(property.IsReadOnly);

            schema.Type = schema.Type.Value & (JSchemaType.String | JSchemaType.Float | JSchemaType.Integer | JSchemaType.Boolean | JSchemaType.Object | JSchemaType.Array);

            //schema.AdditionalProperties.
            //schema. = property.IsReadOnly;

            //if (schema.Enum != null)
            //{
            //    schema.Enum = schema.Enum.Select(x => Enum.getva)
            //}

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

        private string GetPropertyDisplayName(PropertyInfo property) => property.GetCustomAttribute<DisplayAttribute>()?.Name ?? property.Name;
        private string GetPropertyDescription(PropertyInfo property) => property.GetCustomAttribute<DisplayAttribute>()?.Description;

        public string Name { get; }
        public Type FeatureType { get; }
        public Type OptionsType { get; }
        public string Description { get; }
        public FeatureOptionPropertyModel Enabled { get; }
        public IDictionary<string, FeatureOptionPropertyModel> Properties { get; }
        public IEnumerable<FeatureModel> Children { get; }
        public IEnumerable<FeatureDependencyModel> Dependencies { get; }

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
