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

        string Prefix { get; }

        void Save(IFeatureManager manager);
    }

    public class FeatureEditor : IFeatureEditor
    {
        private readonly IEnumerable<FeatureModel> _models;
        private readonly IEnumerable<FeatureGroup> _groups;
        private readonly Func<Type, IAspect> _getFeature;
        private readonly Func<Type, object> _getFeatureOption;
        private readonly JsonSerializer _serializer;

        public FeatureEditor(IEnumerable<FeatureModel> models, IEnumerable<FeatureGroup> groups, Func<Type, IAspect> feature, Func<Type, object> featureOption)
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

        public virtual string Prefix { get { return "Features"; } }

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
            var feature = _getFeature(model.FeatureType);

            object featureOptions = null;
            if (model.OptionsType != null)
                featureOptions = _getFeatureOption(model.FeatureType);

            var json = new JObject();

            json.Add(model.Name, model.Name);

            if (model.HasEnabled)
            {
                if (model.Enabled.OptionsHasIsEnabled)
                    json.Add("enabled", new JValue(model.Enabled.GetValue(featureOptions)));
                else
                    json.Add("enabled", new JValue(model.Enabled.GetValue(feature)));
            }

            if (model.OptionsType != null)
            {
                json.Add("settings", JObject.FromObject(featureOptions, _serializer));
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

        public void Save(IFeatureManager manager)
        {
            var changedItems = _groups.Select(x => SaveModel(x, Model[x.Name])).ToArray();
            var changedFeatures = changedItems.OfType<FeatureSaveContext>();
            var changedOptions = changedItems.OfType<OptionsSaveContext>();

            // Save with feature manager?
        }

        class SaveContext { }
        class FeatureSaveContext : SaveContext
        {
            public FeatureSaveContext(IAspect feature)
            {
                Feature = feature;
            }

            public IAspect Feature { get; }
        }
        class OptionsSaveContext : SaveContext
        {
            public OptionsSaveContext(IAspect feature, object options)
            {
                Feature = feature;
                Options = options;
            }

            public IAspect Feature { get; }
            public object Options { get; }
        }

        private IEnumerable<SaveContext> SaveModel(FeatureGroup group, JToken json)
        {
            foreach (var item in group.Items)
            {
                var model = item as FeatureModel;
                if (model != null)
                {
                    foreach (var result in SaveModel(model, json))
                        yield return result;
                }

                var grouping = item as FeatureGroup;
                if (grouping != null)
                {
                    foreach (var result in SaveModel(grouping, json[grouping.Name]))
                        yield return result;
                }
            }
        }

        private IEnumerable<SaveContext> SaveModel(FeatureModel model, JToken json)
        {
            //json.Add(model.Name, model.Name);
            var feature = _getFeature(model.FeatureType);

            object featureOptions = null;
            if (model.OptionsType != null)
                featureOptions = _getFeatureOption(model.FeatureType);

            if (model.HasEnabled && !model.Enabled.IsReadOnly)
            {
                var enabled = json["enabled"].ToObject<bool>(_serializer);
                if (model.Enabled.OptionsHasIsEnabled)
                {
                    var currentEnabled = (bool)model.Enabled.GetValue(featureOptions);
                    if (currentEnabled != enabled)
                    {
                        model.Enabled.SetValue(featureOptions, enabled);
                    }
                }
                else
                {
                    var currentEnabled = (bool)model.Enabled.GetValue(feature);
                    if (currentEnabled != enabled)
                    {
                        model.Enabled.SetValue(feature, enabled);
                        yield return new FeatureSaveContext(feature);
                    }
                }
            }

            if (model.OptionsType != null)
            {
                var currentValue = JObject.FromObject(featureOptions, _serializer);
                var settings = json["settings"];

                if (!JToken.DeepEquals(currentValue, settings))
                {
                    _serializer.Populate(settings.CreateReader(), featureOptions);
                    yield return new OptionsSaveContext(feature, featureOptions);
                }
            }

            if (model.Children.Any())
            {
                foreach (var child in model.Children)
                {
                    SaveModel(child, json[child.Name]);
                }
            }
        }
    }
}
