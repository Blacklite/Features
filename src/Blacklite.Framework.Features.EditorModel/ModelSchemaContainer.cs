using System;
using System.Collections.Generic;
using System.Linq;
using Temp.Newtonsoft.Json.Linq;
using Temp.Newtonsoft.Json.Schema;
using Temp.Newtonsoft.Json.Schema.Generation;

namespace Blacklite.Framework.Features.EditorModel
{
    class ModelSchemaContainer
    {
        private readonly IEnumerable<FeatureModel> _models;
        private readonly IEnumerable<FeatureModel> _rootModels;
        private readonly IDictionary<string, JSchema> _optionSchemas;
        private readonly IDictionary<string, JSchema> _modelSchemas;
        private readonly JSchemaGenerator _schemaGenerator;
        private readonly JObject _definitions;
        private readonly JSchema _schema;

        public ModelSchemaContainer(JSchema master, IEnumerable<FeatureModel> models, IEnumerable<FeatureModel> rootModels)
        {
            _schema = master;
            _models = models;
            _modelSchemas = new Dictionary<string, JSchema>();
            _optionSchemas = new Dictionary<string, JSchema>();
            _definitions = new JObject();

            _schemaGenerator = new JSchemaGenerator();
            _schemaGenerator.GenerationProviders.Add(new StringEnumGenerationProvider());
        }

        private bool _generatedSchema = false;
        private void GenerateSchema()
        {
            var allModels = new List<FeatureModel>();
            var currentModels = _models.Where(x => _rootModels.Any(z => z.Name == x.Name)).ToArray();
            while (currentModels.Any())
            {
                foreach (var model in currentModels)
                {
                    allModels.Add(model);
                }

                currentModels = currentModels.SelectMany(x => x.Children).ToArray();
            }

            _schema.ExtensionData["definitions"] = _definitions;
            foreach (var model in allModels.AsEnumerable().Reverse())
            {
                GetSchema(model);
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

        private JSchema GetSchema(FeatureModel model)
        {
            JSchema schema;
            if (!_modelSchemas.TryGetValue(model.Name, out schema))
            {
                schema = GetModelSchema(model);
                _modelSchemas.Add(model.Name, schema);
            }

            return schema;
        }

        private JSchema GetOptions(FeatureModel model)
        {
            JSchema options = null;
            if (model.OptionsType != null)
            {
                JToken bogus;
                options = _schemaGenerator.Generate(model.OptionsType);
                if (!_definitions.TryGetValue(model.OptionsType.Name, out bogus))
                    _definitions.Add(model.OptionsType.Name, options);
            }

            return options;
        }

        private JSchema GetFeatureSchema(FeatureModel model)
        {
            var schema = new JSchema();

            JToken bogus;
            if (!_definitions.TryGetValue(model.Name, out bogus))
                _definitions.Add(model.Name, schema);

            schema.Type = JSchemaType.Object;
            schema.Title = model.Name;
            schema.Description = model.Description;
            schema.Format = "grid";
            schema.ExtensionData["options"] = JObject.FromObject(new { disable_collapse = true });

            var hidden = new JSchema();
            hidden.Type = JSchemaType.String;
            hidden.ExtensionData["options"] = JObject.FromObject(new { hidden = true });
            schema.Properties.Add(model.Name, hidden);

            schema.AllowAdditionalProperties = false;
            schema.AllowAdditionalItems = false;

            return schema;
        }

        private void AddFeatureProperties(FeatureModel model, JSchema schema)
        {
            schema.Properties.Add("Enabled", GetOrUpdatePropertySchema(model.Enabled));
        }

        private void AddOptions(FeatureModel model, JSchema schema, JSchema options)
        {
            if (options != null)
            {
                _optionSchemas.Add(model.OptionsType.Name, options);
                options.ExtensionData["options"] = JObject.FromObject(new { collapsed = true });
                foreach (var property in options.Properties)
                {
                    GetOrUpdatePropertySchema(model.Properties[property.Key], property.Value);
                }
                schema.Properties.Add("Settings", options);
            }
        }

        private void AddChildren(FeatureModel model, JSchema schema)
        {
            if (model.Children.Any())
            {
                foreach (var child in model.Children)
                {
                    schema.Properties.Add(child.Name, GetSchema(child));
                }
            }
        }

        private JSchema GetModelSchema(FeatureModel model)
        {
            var options = GetOptions(model);

            var schema = GetFeatureSchema(model);
            AddFeatureProperties(model, schema);
            AddOptions(model, schema, options);
            AddChildren(model, schema);

            return schema;
        }

        private JSchema GetOrUpdatePropertySchema(FeatureOptionPropertyModel property, JSchema schema = null)
        {
            // change this
            schema = schema ?? _schemaGenerator.Generate(property.Type);
            schema.Description = property.Description;

            schema.ExtensionData["readonly"] = new JValue(property.IsReadOnly);

            schema.Type = schema.Type.Value & (JSchemaType.String | JSchemaType.Float | JSchemaType.Integer | JSchemaType.Boolean | JSchemaType.Object | JSchemaType.Array);

            return schema;
        }
    }
}
