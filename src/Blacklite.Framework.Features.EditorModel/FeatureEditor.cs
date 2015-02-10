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
            var property = base.CreateProperty(member, memberSerialization);
            return property;
        }
    }

    class ModelSchemaContainer
    {
        private readonly IEnumerable<FeatureModel> _models;
        private readonly IDictionary<string, JSchema> _optionSchemas;
        private readonly IDictionary<string, JSchema> _modelSchemas;
        private readonly JSchemaGenerator _schemaGenerator;
        private readonly JObject _definitions;
        private readonly JSchema _schema;
        public ModelSchemaContainer(JSchema master, IEnumerable<FeatureModel> models, IEnumerable<FeatureModel> rootModels, IContractResolver resolver)
        {
            _schema = master;
            _models = models;
            _modelSchemas = new Dictionary<string, JSchema>();
            _optionSchemas = new Dictionary<string, JSchema>();

            _schemaGenerator = new JSchemaGenerator()
            {
                ContractResolver = resolver,
            };
            _schemaGenerator.GenerationProviders.Add(new StringEnumGenerationProvider());

            var allModels = new List<FeatureModel>();
            var currentModels = models.Where(x => rootModels.Any(z => z.Name == x.Name)).ToArray();
            while (currentModels.Any())
            {
                foreach (var model in currentModels)
                {
                    allModels.Add(model);
                }

                currentModels = currentModels.SelectMany(x => x.Children).ToArray();
            }

            var definitions = _definitions = new JObject();
            master.ExtensionData["definitions"] = definitions;
            foreach (var model in allModels.AsEnumerable().Reverse())
            {
                //foreach (var model in _schemaContainer.OptionSchemas.Union(_schemaContainer.Schemas))
                GetSchema(model);
            }

        }

        public IEnumerable<FeatureModel> Models { get { return _models; } }
        public IEnumerable<KeyValuePair<string, JSchema>> Schemas { get { return _modelSchemas; } }
        public IEnumerable<KeyValuePair<string, JSchema>> OptionSchemas { get { return _optionSchemas; } }

        public JSchema GetSchema(FeatureModel model)
        {
            JSchema schema;
            if (!_modelSchemas.TryGetValue(model.Name, out schema))
            {
                schema = GetModelSchema(model);
                _modelSchemas.Add(model.Name, schema);
            }

            return schema;
        }

        private JSchema GetModelSchema(FeatureModel model)
        {
            var schema = new JSchema();

            JToken bogus;

            if (model.OptionsType != null)
            {
                var options = _schemaGenerator.Generate(model.OptionsType);
                if (!_definitions.TryGetValue(model.OptionsType.Name, out bogus))
                    _definitions.Add(model.OptionsType.Name, options);

                _optionSchemas.Add(model.OptionsType.Name, options);
                //options.Title = "Options";
                options.ExtensionData["options"] = JObject.FromObject(new { collapsed = true });
                foreach (var property in options.Properties)
                {
                    GetOrUpdatePropertySchema(model.Properties[property.Key], property.Value);
                }
                schema.Properties.Add("Settings", options);
            }

            if (!_definitions.TryGetValue(model.Name, out bogus))
                _definitions.Add(model.Name, schema);

            schema.Type = JSchemaType.Object;
            schema.Title = model.Name;
            schema.Description = model.Description;
            schema.Format = "grid";
            schema.ExtensionData["options"] = JObject.FromObject(new { disable_collapse = true });
            //schema.AllowAdditionalItems = false;

            var hidden = new JSchema();
            hidden.Type = JSchemaType.String;
            //hidden.Enum.Add(new JValue(model.Name));
            hidden.ExtensionData["options"] = JObject.FromObject(new { hidden = true });
            schema.Properties.Add(model.Name, hidden);
            schema.Properties.Add("Enabled", GetOrUpdatePropertySchema(model.Enabled));

            //var constraint = new JSchema();
            //schema.AllOf.Add(constraint);
            //var constraintProperty = new JSchema();
            //constraintProperty.Enum.Add(new JValue(model.Name));
            //constraint.Properties.Add("$feature", constraintProperty);

            if (model.Children.Any())
            {
                var childrenSchema = new JSchema();
                schema.Properties.Add("children", childrenSchema);
                childrenSchema.Title = "";

                childrenSchema.Type = JSchemaType.Array;
                var itemSchema = new JSchema();
                childrenSchema.Items.Add(itemSchema);
                //itemSchema.Type = JSchemaType.Object;
                //itemSchema.Title = "General";

                foreach (var child in model.Children.Select(GetSchema))
                {
                    itemSchema.AnyOf.Add(child);
                }
            }

            schema.AllowAdditionalProperties = false;
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

    public class FeatureEditor
    {
        private readonly IEnumerable<FeatureModel> _models;
        private readonly IEnumerable<FeatureModel> _rootModels;
        private readonly Func<Type, IFeature> _getFeature;
        private readonly Func<Type, object> _getFeatureOption;
        private readonly JsonSerializer _serializer;
        private readonly ModelSchemaContainer _schemaContainer;
        private readonly IContractResolver _resolver;

        public FeatureEditor(IEnumerable<FeatureModel> models, IEnumerable<FeatureModel> rootModels, Func<Type, IFeature> feature, Func<Type, object> featureOption)
        {
            _models = models;
            _rootModels = rootModels;
            _getFeature = feature;
            _getFeatureOption = featureOption;

            var contractResolver = _resolver = new SchemaContractResolver();
            _serializer = new JsonSerializer()
            {
                ContractResolver = contractResolver,
            };

            _serializer.Converters.Add(new StringEnumConverter());
            //_modelSchema = _models.ToDictionary(model => model.Name, model => GetModelSchema(model));
        }

        private JSchema _schema;
        public JSchema Schema
        {
            get
            {
                return _schema ?? (_schema = GenerateSchema());
            }
        }

        private JToken _model;
        public JToken Model
        {
            get
            {
                return _model ?? (_model = GenerateJObject());
            }
        }

        private JToken GenerateJObject()
        {
            //var json = new JObject();
            //foreach (var model in _models)
            //{
            //    json.Add(model.Name, GetModelJObject(model));
            //    //model.Properties.Add(model.Name, GetModelSchema(model));
            //}
            var json = new JObject();
            var jarray = new JArray();
            json.Add("General", jarray);
            foreach (var model in _rootModels)
            {
                jarray.Add(GetModelJObject(model));
                //model.Properties.Add(model.Name, GetModelSchema(model));
            }
            return json;
        }

        private JObject GetModelJObject(FeatureModel model)
        {
            var json = new JObject();

            json.Add(model.Name, model.Name);

            if (model.Enabled.OptionsHasIsEnabled)
                json.Add("Enabled", new JValue(model.Enabled.GetValue(_getFeatureOption(model.FeatureType))));
            else
                json.Add("Enabled", new JValue(model.Enabled.GetValue(_getFeature(model.FeatureType))));

            if (model.OptionsType != null)
            {
                json.Add("Settings", JObject.FromObject(_getFeatureOption(model.FeatureType), _serializer));
            }

            if (model.Children.Any())
            {
                var children = new JArray();
                json.Add("children", children);
                foreach (var child in model.Children)
                {
                    children.Add(GetModelJObject(child));
                }
            }

            return json;
        }

        private JSchema GenerateSchema()
        {
            var schema = new JSchema();
            schema.Type = JSchemaType.Object;
            schema.ExtensionData["options"] = JObject.FromObject(new { disable_collapse = true });
            schema.Title = "Features";

            var schemaContainer = new ModelSchemaContainer(schema, _models, _rootModels, _resolver);



            var group = new JSchema();
            group.Title = "General";
            group.Type = JSchemaType.Array;
            group.Format = "tabs";
            group.ExtensionData["options"] = JObject.FromObject(new { disable_collapse = true });
            schema.Properties.Add("General", group);

            var itemSchema = new JSchema();
            group.Items.Add(itemSchema);
            //itemSchema.Type = JSchemaType.Object;
            //itemSchema.Title = "General";

            foreach (var model in schemaContainer.Schemas.Where(x => _rootModels.Any(z => z.Name == x.Key)).OrderBy(x => x.Key).Select(x => x.Value))
            {

                //var constraint = new JSchema();
                //schema.AllOf.Add(constraint);
                //var constraintProperty = new JSchema();
                //constraintProperty.Enum.Add(new JValue(model.Name));
                //constraint.Properties.Add("$feature", constraintProperty);

                //var newSchema = new JSchema();
                //newSchema.

                itemSchema.AnyOf.Add(model);
            }

            //var modelSchemas = _models.Select(model => GetModelSchema(model));
            //schema.ExtensionData["definitions"] = JObject.FromObject(modelSchemas.ToDictionary(x => x.Title));

            //schema.Type = JSchemaType.Array;
            //schema.Format = "table";

            //var items = new JSchema();
            //schema.Items.Add(items);
            //items.Title = "General";
            //items.Format = "tabs";
            //foreach (var model in modelSchemas)
            //    items.OneOf.Add(model);

            //foreach (var model in _models)
            //{
            //    schema.Items.Add(GetModelSchema(model));
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


        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return $"Editor for {this.Name}";
        }
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
