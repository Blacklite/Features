using Blacklite.Framework.Features.Editors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Blacklite.Framework.Features.Editors.Schema
{
    public interface ISchemaContainer
    {
        JSchema Schema { get; }
    }

    class SchemaContainer : ISchemaContainer
    {
        private readonly IEnumerable<EditorModel> _models;
        private readonly IEnumerable<EditorGroupOrModel> _groups;
        private readonly IDictionary<string, JSchema> _optionSchemas;
        private readonly IDictionary<string, JSchema> _modelSchemas;
        private readonly JSchemaGenerator _schemaGenerator;
        private Func<Type, JSchema> _generateMethod;
        private readonly JsonSerializer _serializer;
        private readonly JObject _definitions;
        private readonly JSchema _schema;

        public SchemaContainer(JSchema master, IEnumerable<EditorModel> models, IEnumerable<EditorGroupOrModel> groups)
        {
            _schema = master;
            _models = models;
            _groups = groups;
            _modelSchemas = new Dictionary<string, JSchema>();
            _optionSchemas = new Dictionary<string, JSchema>();
            _definitions = new JObject();

            _schemaGenerator = new JSchemaGenerator();
            _schemaGenerator.GenerationProviders.Add(new StringEnumGenerationProvider());
            _schemaGenerator.ContractResolver = new CamelCasePropertyNamesContractResolver();

            _serializer = new JsonSerializer();
            _serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            _serializer.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            _generateMethod = (type) =>
            {
                var method = typeof(JSchemaGenerator).GetTypeInfo().GetDeclaredMethod("GenerateInternal");
                return (JSchema)method.Invoke(_schemaGenerator, new object[] { type, Required.Always, null, null });
            };
        }

        private IEnumerable<EditorModel> GetOrderedEditorModels(IEnumerable<EditorGroupOrModel> gom)
        {
            foreach (var item in gom)
            {
                var model = item as EditorModel;
                if (model != null)
                {
                    if (model.Children?.Any() ?? false)
                    {
                        foreach (var result in GetOrderedEditorModels(model.Children))
                            yield return result;
                    }

                    yield return model;
                }

                var group = item as EditorGroup;
                if (group != null)
                {
                    foreach (var result in GetOrderedEditorModels(group.Items))
                    {
                        yield return result;
                    }
                }
            }
        }

        private bool _generatedSchema = false;
        private void GenerateSchema()
        {
            var orderedModels = GetOrderedEditorModels(_groups).ToArray();
            _schema.ExtensionData["definitions"] = _definitions;
            foreach (var model in orderedModels)
            {
                GetSchema(model);
            }

            foreach (var rootModel in _groups)
            {
                var model = rootModel as EditorModel;
                if (model != null)
                    _schema.Properties.Add(model.Name, GetSchema(model));

                var group = rootModel as EditorGroup;
                if (group != null)
                    _schema.Properties.Add(group.Name, GetSchema(group));
            }

            _generatedSchema = true;
        }

        public JSchema Schema
        {
            get
            {
                if (!_generatedSchema)
                    GenerateSchema();
                return _schema;
            }
        }

        private JSchema GetSchema(EditorGroup group)
        {
            return GetGroupSchema(group);
        }

        private JSchema GetSchema(EditorModel model)
        {
            JSchema schema;
            if (!_modelSchemas.TryGetValue(model.Name, out schema))
            {
                schema = GetModelSchema(model);
                _modelSchemas.Add(model.Name, schema);
            }

            return schema;
        }

        private JSchema GetOptions(EditorModel model)
        {
            JSchema options = null;
            if (model.HasOptions && !model.OptionsIsFeature)
            {
                JToken bogus;
                options = _generateMethod(model.OptionsType);
                options.Title = "Settings";
                options.Format = FeatureEditor.OptionsKey;

                if (!_definitions.TryGetValue(model.OptionsName, out bogus))
                    _definitions.Add(model.OptionsName, options);
            }
            else if (model.HasOptions && model.OptionsIsFeature && model.OptionsFeature != null)
            {
                // Produce editor for feature options....
                options = GetSchema(model.OptionsFeature);
                options.Format = "feature-inline";
                if (options.Properties.ContainsKey("isEnabled"))
                {
                    options.Properties["isEnabled"].Format = "inline";
                    //options.Properties["isEnabled"].Title = options.Title;
                }
            }

            if (options != null)
            {
                options.Description = model.OptionsDescription;
                if (options.ExtensionData.ContainsKey(FeatureEditor.OptionsKey))
                    options.ExtensionData[FeatureEditor.OptionsKey]["showHeader"] = new JValue(false);
                else
                    options.ExtensionData[FeatureEditor.OptionsKey] = JObject.FromObject(new { showHeader = false });
            }

            return options;
        }

        private JSchema GetFeatureSchema(EditorModel model)
        {
            var schema = new JSchema();

            JToken bogus;
            if (!_definitions.TryGetValue(model.Name, out bogus))
                _definitions.Add(model.Name, schema);

            schema.Type = JSchemaType.Object;
            schema.Title = model.Title;
            schema.Description = model.Description;
            //schema.ExtensionData[FeatureEditor.OptionsKey] = JObject.FromObject(new { showHeader = false });
            schema.Format = "feature";
            schema.ExtensionData[FeatureEditor.OptionsKey] = JObject.FromObject(new { key = model.Name }, _serializer);
            schema.ExtensionData["requires"] = JObject.FromObject(
                model.Describer.DependsOn.ToDictionary(
                    x => $"{x.Key.Type.Name.CamelCase()}.isEnabled",
                    x => new
                    {
                        requiredValue = x.Value,
                        title = x.Key.DisplayName
                    })
            );

            schema.AllowAdditionalProperties = false;
            schema.AllowAdditionalItems = false;

            return schema;
        }

        private void AddFeatureProperties(EditorModel model, JSchema schema)
        {
            if (model.HasEnabled)
            {
                schema.Properties.Add("isEnabled", GetEnabledPropertySchema(model));
            }

            if (model.HasProperties)
            {
                var propertySchema = _generateMethod(model.FeatureType);
                //_optionSchemas.Add(model.Name, propertySchema);
                propertySchema.Title = model.Title;
                if (propertySchema.ExtensionData.ContainsKey(FeatureEditor.OptionsKey))
                    propertySchema.ExtensionData[FeatureEditor.OptionsKey]["showHeader"] = new JValue(false);
                else
                    propertySchema.ExtensionData[FeatureEditor.OptionsKey] = JObject.FromObject(new { showHeader = false });
                propertySchema.Format = FeatureEditor.OptionsKey;

                if (model.HasEnabled && propertySchema.Properties.ContainsKey("isEnabled"))
                    propertySchema.Properties.Remove("isEnabled");
                if (model.HasOptions && propertySchema.Properties.ContainsKey("options"))
                    propertySchema.Properties.Remove("options");

                foreach (var property in propertySchema.Properties)
                {
                    GetOrUpdatePropertySchema(model.Properties[property.Key], property.Value);
                }
                schema.Properties.Add(FeatureEditor.SettingsKey, propertySchema);
            }
        }

        private void AddOptions(EditorModel model, JSchema schema, JSchema options)
        {
            if (options != null && !model.OptionsIsFeature)
            {
                options.Title = "Settings";
                _optionSchemas.Add(model.OptionsName, options);
                foreach (var property in options.Properties)
                {
                    GetOrUpdatePropertySchema(model.Options[property.Key], property.Value);
                }
                schema.Properties.Add(FeatureEditor.OptionsKey, options);
            }
            else if (options != null && model.OptionsIsFeature)
            {
                _optionSchemas.Add(model.OptionsName, options);
                schema.Properties.Add(FeatureEditor.OptionsKey, options);
            }
        }

        private void AddChildren(EditorModel model, JSchema schema)
        {
            if (model.Children.Any())
            {
                foreach (var child in model.Children)
                {
                    var childSchema = GetSchema(child);
                    schema.Properties.Add(child.Name, childSchema);
                    childSchema.ExtensionData["child"] = JToken.FromObject(true);
                }
            }
        }

        private JSchema GetGroupSchema(EditorGroup group)
        {
            var schema = new JSchema();
            schema.Type = JSchemaType.Object;
            schema.Title = group.Title;
            if (!group.Items.Any(z => z is EditorModel))
                schema.Format = "tabs";
            else
            {
                schema.Format = "rows";
                schema.ExtensionData[FeatureEditor.OptionsKey] = JObject.FromObject(new { showHeader = false });
            }

            foreach (var item in group.Items)
            {
                var model = item as EditorModel;
                if (model != null)
                {
                    schema.Properties.Add(item.Name, GetSchema(model));
                }

                var grouping = item as EditorGroup;
                if (grouping != null)
                {
                    schema.Properties.Add(grouping.Name, GetSchema(grouping));
                }
            }

            return schema;
        }

        private JSchema GetModelSchema(EditorModel model)
        {
            var options = GetOptions(model);

            var schema = GetFeatureSchema(model);
            AddFeatureProperties(model, schema);
            AddOptions(model, schema, options);
            AddChildren(model, schema);

            return schema;
        }

        private JSchema GetOrUpdatePropertySchema(EditorOptionPropertyModel property, JSchema schema = null)
        {
            // change this
            schema = schema ?? _generateMethod(property.Type);
            schema.Title = property.Title;
            schema.Description = property.Description;

            schema.ExtensionData["readonly"] = new JValue(property.IsReadOnly);

            schema.Type = schema.Type.Value & (JSchemaType.String | JSchemaType.Number | JSchemaType.Integer | JSchemaType.Boolean | JSchemaType.Object | JSchemaType.Array);

            return schema;
        }

        private JSchema GetEnabledPropertySchema(EditorModel model)
        {
            var schema = _generateMethod(model.Enabled.Type);
            schema.Title = model.Enabled.Title;
            schema.Description = model.Description;

            schema.ExtensionData["readonly"] = new JValue(model.Enabled.IsReadOnly);

            schema.Type = schema.Type.Value & (JSchemaType.String | JSchemaType.Number | JSchemaType.Integer | JSchemaType.Boolean | JSchemaType.Object | JSchemaType.Array);

            return schema;
        }
    }
}
