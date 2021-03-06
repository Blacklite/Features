using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Editors.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Blacklite.Framework.Features.Editors
{
    static class StringExtension
    {
        internal static string CamelCase(this string value)
        {
            return value.Substring(0, 1).ToLowerInvariant() + value.Substring(1).Replace(" ", "");
        }
    }

    public class EditorValue
    {
        public bool? IsEnabled { get; set; }
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
        public IDictionary<string, object> Options { get; } = new Dictionary<string, object>();
    }

    public interface IFeatureEditor
    {
        IEnumerable<GroupOrModel> Groups { get; }

        IDictionary<string, EditorValue> Values { get; }

        JToken JToken { get; }

        IEnumerable<Model> Models { get; }

        string Prefix { get; }

        void Save();
    }

    public class FeatureEditor : IFeatureEditor
    {
        private readonly IEnumerable<Model> _models;
        private readonly IEnumerable<GroupOrModel> _groups;
        private readonly IDictionary<string, EditorValue> _values;
        private readonly Func<Type, IFeature> _getFeature;
        private readonly Func<Type, object> _getFeatureOption;
        private readonly JsonSerializer _serializer;
        private readonly IFeatureManager _featureManager;

        public const string SettingsKey = "settings";
        public const string OptionsKey = "options";

        public FeatureEditor(IFeatureManager featureManager, IEnumerable<Model> models, IEnumerable<GroupOrModel> groups, Func<Type, IFeature> feature, Func<Type, object> featureOption)
        {
            _featureManager = featureManager;
            _models = models;
            _groups = groups;
            _values = new Dictionary<string, EditorValue>();
            _getFeature = feature;
            _getFeatureOption = featureOption;

            _serializer = new JsonSerializer();
            _serializer.Converters.Add(new StringEnumConverter());
            _serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //_modelSchema = _models.ToDictionary(model => model.Name, model => GetModelSchema(model));
        }

        public IEnumerable<GroupOrModel> Groups { get { return _groups; } }

        public IEnumerable<Model> Models { get { return _models; } }

        public IDictionary<string, EditorValue> Values
        {
            get
            {
                if (_model == null)
                    _model = GenerateJObject();
                return _values;
            }
        }

        private JToken _model;
        public JToken JToken
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
            foreach (var model in _models)
            {
                json.Add(model.Name, GetModelJObject(model));
            }
            return json;
        }

        private JObject GetModelJObject(Model model)
        {
            var feature = _getFeature(model.FeatureType);

            object featureOptions = null;
            if (model.OptionsType != null)
                featureOptions = _getFeatureOption(model.FeatureType);

            var json = new JObject();
            var value = new EditorValue();
            _values.Add(model.Name, value);

            //json.Add(model.Name, model.Name);

            if (model.HasEnabled)
            {
                var enabled = (bool)model.Enabled.GetValue(feature);
                value.IsEnabled = enabled;
                json.Add("isEnabled", new JValue(enabled));
            }

            if (model.HasProperties)
            {
                var settings = new JObject();
                foreach (var property in model.Properties)
                {
                    var pv = property.Value.GetValue(feature);
                    value.Properties.Add(property.Key, pv);
                    settings.Add(property.Key.CamelCase(), new JValue(pv));
                }
                json.Add(FeatureEditor.SettingsKey, settings);
            }

            if (model.HasOptions && !model.OptionsIsFeature)
            {
                foreach (var option in model.Options)
                {
                    var pv = option.Value.GetValue(featureOptions);
                    value.Options.Add(option.Key, pv);
                }
                json.Add(FeatureEditor.OptionsKey, JObject.FromObject(featureOptions, _serializer));
            }

            return json;
        }

        public void Save()
        {
            var changedItems = _models.SelectMany(x => SaveModel(x, JToken[x.Name])).ToArray();

            foreach (var item in changedItems)
            {
                // Save with feature manager?
                if (!_featureManager.TrySaveFeature(item.Describer, item.Feature))
                {
                    // logging? exception? error?
                }
            }
        }

        class SaveContext
        {
            public SaveContext(IFeature feature, IFeatureDescriber describer)
            {
                Feature = feature;
                Describer = describer;
            }

            public IFeature Feature { get; }
            public IFeatureDescriber Describer { get; }
        }

        private IEnumerable<SaveContext> SaveModel(Group group, JToken json)
        {
            foreach (var item in group.Items)
            {
                var model = item as Model;
                if (model != null)
                {
                    foreach (var result in SaveModel(model, json[model.Name]))
                        yield return result;
                }

                var grouping = item as Group;
                if (grouping != null)
                {
                    foreach (var result in SaveModel(grouping, json[grouping.Name]))
                        yield return result;
                }
            }
        }

        private bool JValueEqual(JValue left, JValue right)
        {
            if (left?.Value == null && right?.Value == null)
                return true;

            if (left?.Value == null && right?.Value != null)
                return false;

            if (left?.Value != null && right?.Value == null)
                return false;

            return left.Value.Equals(right.Value);
        }

        private IEnumerable<SaveContext> SaveModel(Model model, JToken json)
        {
            //json.Add(model.Name, model.Name);
            var feature = _getFeature(model.FeatureType);
            var featureChanged = false;

            if (model.HasEnabled && !model.Enabled.IsReadOnly)
            {
                var enabled = json["isEnabled"].ToObject<bool>(_serializer);
                var currentEnabled = (bool)model.Enabled.GetValue(feature);
                if (!currentEnabled.Equals(enabled))
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

                if (model.HasEnabled && currentValue.ContainsKey("isEnabled"))
                    currentValue.Remove("isEnabled");

                var settings = json[FeatureEditor.SettingsKey] as IDictionary<string, JToken>;

                foreach (var item in currentValue
                    .Join(settings, x => x.Key, x => x.Key, (current, setting) => new { current, setting })
                    .Join(model.Properties, x => x.current.Key, x => x.Key.CamelCase(), (anon, property) => new { key = anon.current.Key, current = anon.current.Value as JValue, setting = anon.setting.Value as JValue, property = property.Value })
                    .Where(x => !x.property.IsReadOnly)
                    .Where(x => !JValueEqual(x.current, x.setting)))
                {
                    if (item.setting.Type == JTokenType.Null)
                        item.property.SetValue(feature, null);
                    else
                        item.property.SetValue(feature, item.setting.ToObject(item.property.Type));
                    featureChanged = true;
                }
            }

            IFeature optionFeature = null;

            if (model.HasOptions)
            {
                var featureOptions = _getFeatureOption(model.FeatureType);
                var currentValue = JObject.FromObject(featureOptions, _serializer) as IDictionary<string, JToken>;
                IDictionary<string, JToken> settings = null;
                if (model.OptionsIsFeature && model.OptionsFeature != null)
                    settings = JToken[model.OptionsFeature.Name] as IDictionary<string, JToken>;
                else if (!model.OptionsIsFeature)
                    settings = json[FeatureEditor.OptionsKey] as IDictionary<string, JToken>;

                if (settings != null)
                {
                    foreach (var item in currentValue
                        .Join(settings, x => x.Key, x => x.Key, (current, setting) => new { current, setting })
                        .Join(model.Options, x => x.current.Key, x => x.Key.CamelCase(), (anon, property) => new { key = anon.current.Key, current = currentValue[anon.current.Key] as JValue, setting = settings[anon.setting.Key] as JValue, property = property.Value })
                        .Where(x => !x.property.IsReadOnly)
                        .Where(x => !JValueEqual(x.current, x.setting)))
                    {
                        if (item.setting.Type == JTokenType.Null)
                            item.property.SetValue(featureOptions, null);
                        else
                            item.property.SetValue(featureOptions, item.setting.ToObject(item.property.Type));

                        if (!model.OptionsIsFeature)
                        {
                            featureChanged = true;
                        }
                        else
                        {
                            optionFeature = (IFeature)featureOptions;
                        }
                    }
                }
            }

            if (featureChanged)
                yield return new SaveContext(feature, model.Describer);

            if (optionFeature != null)
                yield return new SaveContext(optionFeature, model.OptionsFeature.Describer);

            //if (model.Children.Any())
            //{
            //    foreach (var child in model.Children)
            //    {
            //        foreach (var item in SaveModel( child, json[child.Name]))
            //            yield return item;
            //    }
            //}
        }
    }

    public interface IFeatureEditor<TFactory> : IFeatureEditor
        where TFactory : IFeatureEditorFactory
    {
    }

    public class FeatureEditor<TFactory> : IFeatureEditor, IFeatureEditor<TFactory>
        where TFactory : IFeatureEditorFactory
    {
        private readonly IFeatureEditor _editor;

        public FeatureEditor(TFactory factory)
        {
            _editor = factory.GetFeatureEditor();
        }

        public IEnumerable<GroupOrModel> Groups { get { return _editor.Groups; } }

        public IEnumerable<Model> Models { get { return _editor.Models; } }

        public IDictionary<string, EditorValue> Values { get { return _editor.Values; } }

        public JToken JToken
        {
            get
            {
                return _editor.JToken;
            }
        }

        public string Prefix
        {
            get
            {
                return _editor.Prefix;
            }
        }

        public void Save()
        {
            _editor.Save();
        }
    }
}
