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
    static class StringExtension
    {
        internal static string CamelCase(this string value)
        {
            return value.Substring(0, 1).ToLowerInvariant() + value.Substring(1);
        }
    }

    public interface IFeatureEditor
    {
        JSchema Schema { get; }

        JToken Model { get; }
    }

    public class FeatureEditor : IFeatureEditor
    {
        private readonly IEnumerable<FeatureModel> _models;
        private readonly IEnumerable<FeatureGroup> _groups;
        private readonly Func<Type, IFeature> _getFeature;
        private readonly Func<Type, object> _getFeatureOption;
        private readonly JsonSerializer _serializer;
        private readonly ModelSchemaContainer _schemaContainer;

        public FeatureEditor(IEnumerable<FeatureModel> models, IEnumerable<FeatureGroup> groups, Func<Type, IFeature> feature, Func<Type, object> featureOption)
        {
            _models = models;
            _groups = groups;
            _getFeature = feature;
            _getFeatureOption = featureOption;

            _serializer = new JsonSerializer();
            _serializer.Converters.Add(new StringEnumConverter());
            _serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
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
            foreach (var model in _groups)
            {
                json.Add(model.Name, GetModelJObject(model));
            }
            return json;
        }

        private JObject GetModelJObject(FeatureGroup group)
        {
            var json = new JObject();
            foreach (var item in group.Items)
            {
                var model = item as FeatureModel;
                if (model != null)
                {
                    json.Add(model.Name, GetModelJObject(model));
                }

                var grouping = item as FeatureGroup;
                if (grouping != null)
                {
                    json.Add(grouping.Name, GetModelJObject(grouping));
                }
            }

            return json;
        }
        private JObject GetModelJObject(FeatureModel model)
        {
            var json = new JObject();

            json.Add(model.Name, model.Name);

            if (model.Enabled.OptionsHasIsEnabled)
                json.Add("enabled", new JValue(model.Enabled.GetValue(_getFeatureOption(model.FeatureType))));
            else
                json.Add("enabled", new JValue(model.Enabled.GetValue(_getFeature(model.FeatureType))));

            if (model.OptionsType != null)
            {
                json.Add("settings", JObject.FromObject(_getFeatureOption(model.FeatureType), _serializer));
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
            schema.Format = "tabs";

            var schemaContainer = new ModelSchemaContainer(schema, _models, _groups);

            return schemaContainer.Schema;
        }
    }
}
