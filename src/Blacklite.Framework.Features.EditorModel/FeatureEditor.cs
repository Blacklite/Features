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
using Microsoft.AspNet.Mvc.ModelBinding;

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

        void Save(ModelStateDictionary modelState);
    }

    public class FeatureEditor : IFeatureEditor
    {
        private readonly IEnumerable<FeatureModel> _models;
        private readonly IEnumerable<FeatureGroup> _groups;
        private readonly Func<Type, IFeature> _getFeature;
        private readonly Func<Type, object> _getFeatureOption;
        private readonly JsonSerializer _serializer;
        private readonly IFeatureManager _featureManager;

        public const string SettingsKey = "settings";
        public const string OptionsKey = "options";

        public FeatureEditor(IFeatureManager featureManager, IEnumerable<FeatureModel> models, IEnumerable<FeatureGroup> groups, Func<Type, IFeature> feature, Func<Type, object> featureOption)
        {
            _featureManager = featureManager;
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
                json.Add("enabled", new JValue(model.Enabled.GetValue(feature)));
            }

            if (model.HasProperties)
            {
                var settings = new JObject();
                foreach (var property in model.Properties)
                {
                    settings.Add(property.Key.CamelCase(), new JValue(property.Value.GetValue(feature)));
                }
                json.Add(FeatureEditor.SettingsKey, settings);
            }

            if (model.HasOptions && !model.OptionsIsFeature)
            {
                json.Add(FeatureEditor.OptionsKey, JObject.FromObject(featureOptions, _serializer));
            }
            else if (model.HasOptions && model.OptionsIsFeature)
            {
                json.Add(FeatureEditor.OptionsKey, JObject.FromObject(GetModelJObject(model.OptionsFeature), _serializer));
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
            schema.ExtensionData[FeatureEditor.OptionsKey] = JObject.FromObject(new { disable_collapse = true });
            schema.Title = "Features";
            schema.Format = "tabs";

            var schemaContainer = new ModelSchemaContainer(schema, _models, _groups);

            return schemaContainer.Schema;
        }

        public void Save(ModelStateDictionary modelState)
        {
            var changedItems = _groups.SelectMany(x => SaveModel(modelState, x, Model[x.Name])).ToArray();
            var changedFeatures = changedItems.OfType<FeatureSaveContext>();
            var changedOptions = changedItems.OfType<OptionsSaveContext>();

            // Save with feature manager?
        }

        class SaveContext { }
        class FeatureSaveContext : SaveContext
        {
            public FeatureSaveContext(IFeature feature)
            {
                Feature = feature;
            }

            public IFeature Feature { get; }
        }
        class OptionsSaveContext : SaveContext
        {
            public OptionsSaveContext(IFeature feature, object options)
            {
                Feature = feature;
                Options = options;
            }

            public IFeature Feature { get; }
            public object Options { get; }
        }

        private IEnumerable<SaveContext> SaveModel(ModelStateDictionary modelState, FeatureGroup group, JToken json)
        {
            foreach (var item in group.Items)
            {
                var model = item as FeatureModel;
                if (model != null)
                {
                    foreach (var result in SaveModel(modelState, model, json[model.Name]))
                        yield return result;
                }

                var grouping = item as FeatureGroup;
                if (grouping != null)
                {
                    foreach (var result in SaveModel(modelState, grouping, json[grouping.Name]))
                        yield return result;
                }
            }
        }

        private IEnumerable<SaveContext> SaveModel(ModelStateDictionary modelState, FeatureModel model, JToken json)
        {
            //json.Add(model.Name, model.Name);
            var feature = _getFeature(model.FeatureType);
            var featureChanged = false;

            if (model.HasEnabled && !model.Enabled.IsReadOnly)
            {
                var enabled = json["enabled"].ToObject<bool>(_serializer);
                var currentEnabled = (bool)model.Enabled.GetValue(feature);
                if (currentEnabled != enabled)
                {
                    model.Enabled.SetValue(feature, enabled);
                    featureChanged = true;
                }
            }

            if (model.HasProperties)
            {
                var currentValue = JObject.FromObject(feature, _serializer) as IDictionary<string, JToken>;

                if (model.HasOptions && currentValue.ContainsKey(FeatureEditor.OptionsKey))
                    currentValue.Remove(FeatureEditor.OptionsKey);

                if (model.HasEnabled && currentValue.ContainsKey("enabled"))
                    currentValue.Remove("enabled");

                var settings = json[FeatureEditor.SettingsKey] as IDictionary<string, JToken>;

                foreach (var item in currentValue
                    .Join(settings, x => x.Key, x => x.Key, (current, setting) => new { current, setting })
                    .Join(model.Properties, x => x.current.Key, x => x.Key.CamelCase(), (anon, property) => new { key = anon.current.Key, current = anon.current.Value, setting = anon.setting.Value, property = property.Value })
                    .Where(x => !x.property.IsReadOnly))
                {
                    if (item.current != item.setting)
                    {
                        if (item.setting.Type == JTokenType.Null)
                            item.property.SetValue(feature, null);
                        else
                            item.property.SetValue(feature, item.setting.ToObject(item.property.Type));
                        featureChanged = true;
                    }
                }
            }

            if (model.HasOptions)
            {
                var featureOptions = _getFeatureOption(model.FeatureType);
                var currentValue = JObject.FromObject(featureOptions, _serializer) as IDictionary<string, JToken>;
                var settings = json[FeatureEditor.OptionsKey] as IDictionary<string, JToken>;

                foreach (var item in currentValue
                    .Join(settings, x => x.Key, x => x.Key, (current, setting) => new { current, setting })
                    .Join(model.Options, x => x.current.Key, x => x.Key.CamelCase(), (anon, property) => new { key = anon.current.Key, current = currentValue[anon.current.Key], setting = settings[anon.setting.Key], property = property.Value })
                    .Where(x => !x.property.IsReadOnly))
                {
                    if (item.current != item.setting)
                    {
                        if (item.setting.Type == JTokenType.Null)
                            item.property.SetValue(featureOptions, null);
                        else
                            item.property.SetValue(featureOptions, item.setting.ToObject(item.property.Type));
                        featureChanged = true;
                    }
                }
            }

            if (featureChanged)
                yield return new FeatureSaveContext(feature);

            if (model.Children.Any())
            {
                foreach (var child in model.Children)
                {
                    foreach (var item in SaveModel(modelState, child, json[child.Name]))
                        yield return item;
                }
            }
        }
    }
}
