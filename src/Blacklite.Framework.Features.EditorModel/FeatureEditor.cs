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
    public class FeatureEditor
    {
        private readonly IEnumerable<FeatureModel> _models;
        private readonly IEnumerable<FeatureModel> _rootModels;
        private readonly Func<Type, IFeature> _getFeature;
        private readonly Func<Type, object> _getFeatureOption;
        private readonly JsonSerializer _serializer;
        private readonly ModelSchemaContainer _schemaContainer;

        public FeatureEditor(IEnumerable<FeatureModel> models, IEnumerable<FeatureModel> rootModels, Func<Type, IFeature> feature, Func<Type, object> featureOption)
        {
            _models = models;
            _rootModels = rootModels;
            _getFeature = feature;
            _getFeatureOption = featureOption;

            _serializer = new JsonSerializer();
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
            var json = new JObject();
            var group = new JObject();
            json.Add("General", group);
            foreach (var model in _rootModels)
            {
                group.Add(model.Name, GetModelJObject(model));
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
                foreach (var child in model.Children)
                {
                    json.Add(child.Name, GetModelJObject(child));
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

            var schemaContainer = new ModelSchemaContainer(schema, _models, _rootModels);



            var group = new JSchema();
            group.Title = "General";
            group.Type = JSchemaType.Object;
            //group.Format = "tabs";
            group.ExtensionData["options"] = JObject.FromObject(new { disable_collapse = true });
            schema.Properties.Add("General", group);

            foreach (var model in _rootModels.OrderBy(x => x.Name).Select(model => new { model.Name, schema = schemaContainer.GetSchema(model) }))
            {
                group.Properties.Add(model.Name, model.schema);
            }

            return schemaContainer.Schema;
        }
    }
}
