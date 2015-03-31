using Blacklite.Json.Schema;
using System;
using Blacklite.Json.Schema.Editors;
using Temp.Newtonsoft.Json.Schema;
using Microsoft.AspNet.Mvc.Rendering;
using System.Linq;
using Temp.Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;

namespace Blacklite.Framework.Features.Editors.JsonEditors
{
    public class FeatureObjectJsonEditor : JsonEditor
    {
        private readonly IJsonEditorProvider _editorProvider;
        private readonly IFeatureJsonEditorDecorator _featureJsonEditorDecorator;
        public FeatureObjectJsonEditor(
            IJsonEditorResolutionContext context,
            IJsonEditorProvider editorProvider,
            IFeatureJsonEditorDecorator featureJsonEditorDecorator,
            EditorOptions options = null) : base(context, options)
        {
            _editorProvider = editorProvider;
            _featureJsonEditorDecorator = featureJsonEditorDecorator;
        }

        public override JsonEditorRenderer Build()
        {
            var container = new TagBuilder("div");
            container.Attributes.Add("data-editor-type", this.ToString());

            //container = Context.Decorator.DecorateItemContainer(Context, container);

            JsonEditorRenderer enabledRenderer = null;
            JsonEditorRenderer propertiesRenderer = null;
            JsonEditorRenderer settingsRenderer = null;
            if (Context.Schema.Properties.ContainsKey("isEnabled"))
            {
                enabledRenderer = GetPropertyTagBuilder("isEnabled", Context.Schema.Properties["isEnabled"]);
            }

            string javaScript = string.Empty;
            var requires = Context.Schema.ExtensionData["requires"] as IDictionary<string, JToken>;
            var requireObjects = requires?.Select(item => new { id = TagBuilder.CreateSanitizedId($"{Context.Prefix.Split('.')[0]}.{item.Key}", "_"), value = item.Value["requiredValue"].ToString().ToLowerInvariant() })?.ToArray();
            if (requireObjects != null && requireObjects.Any())
            {
                var elementId = TagBuilder.CreateSanitizedId($"{Context.Prefix.Split('.')[0]}.{Context.Options.Key}.isEnabled", "_");
                if (requireObjects.Any())
                {
                    var constraintObjects = string.Join(";\n", requireObjects.Select(item => $" elements['{item.id}']=elements['{item.id}'] || (elements['{item.id}'] = $('#{item.id}'))"));
                    var constraint = string.Join(" && ", requireObjects.Select(item => $"elements['{item.id}'][0].checked == {item.value}"));
                    var constraintSwitch = string.Join(";\n", requireObjects.Select(item => _featureJsonEditorDecorator.EmitRequiredConstraintOnChange(item.id, "checkConstraints")));

                    javaScript = $@"
    (function() {{
        var key = '{elementId}';
        elements[key]=elements[key] || (elements[key] = $('#'+key));
        {constraintObjects};
        {_featureJsonEditorDecorator.EmitRequiredConstraint(elementId, constraint)}
        {constraintSwitch}
        callbacks.push(checkConstraints);
    }})();
";
                }
            }

            var propertyTagRenderes = Context.Schema.Properties
                .Where(x => x.Key != FeatureEditor.SettingsKey && x.Key != FeatureEditor.OptionsKey && x.Key != "isEnabled")
                .Select(property => new { Key = property.Key, Value = GetChildFeatureTagBuilder(property.Key, property.Value) })
                .Where(x => x.Value != null)
                .ToArray();

            string helpTitle = null;
            string helpContent = null;
            if (Context.Schema.ExtensionData.ContainsKey("requires"))
            {
                if (requires != null && requires.Any())
                {
                    helpTitle = "Requires";
                    helpContent = string.Join("<br />\n", requires.Select(x => $"{x.Value["title"]} to be {(x.Value["requiredValue"].ToString() == "True" ? "ON" : "OFF")}"));
                }
            }

            var title = new TagBuilder("div");
            title.InnerHtml = this.GetTitle();
            title = Context.Decorator.DecorateTitle(Context, title);

            container.InnerHtml += title.ToString();
            //ExtensionData["requires"]

            var toggle = new TagBuilder("div");
            _featureJsonEditorDecorator.DecorateFeatureCheckbox(Context, toggle,
                helpTitle: helpTitle,
                helpContent: helpContent);

            var modalTitle = "Settings";
            if (Context.Schema.Properties.ContainsKey(FeatureEditor.SettingsKey))
            {
                modalTitle = "Settings";
                propertiesRenderer = GetSettingsTagBuilder(FeatureEditor.SettingsKey, Context.Schema.Properties[FeatureEditor.SettingsKey]);
            }

            string key = null;
            string childHelpTitle = null;
            string childHelpContent = null;

            if (Context.Schema.Properties.ContainsKey(FeatureEditor.OptionsKey))
            {
                var childSchema = Context.Schema.Properties[FeatureEditor.OptionsKey];
                key = (childSchema.ExtensionData["options"]?["key"]?.ToString() ?? $"{Context.Options.Key}.{FeatureEditor.OptionsKey}");
                //modalTitle = Context.Schema.Title;
                modalTitle = Context.Schema.Properties[FeatureEditor.OptionsKey].Title;
                settingsRenderer = GetFeatureOptionsTagBuilder(key, childSchema);

                if (childSchema.ExtensionData.ContainsKey("requires"))
                {
                    var childRequires = childSchema.ExtensionData["requires"] as IDictionary<string, JToken>;
                    if (childRequires != null && childRequires.Any())
                    {
                        childHelpTitle = "Requires";
                        childHelpContent = string.Join("<br />\n", childRequires.Select(x => $"{x.Value["title"]} to be {(x.Value["requiredValue"].ToString() == "True" ? "ON" : "OFF")}"));
                    }
                }
            }

            return new JsonEditorRenderer(Context.Serializer, v =>
            {
                var value = v[Context.Options.Key];
                var result = new TagBuilder("div");
                result.MergeAttributes(container.Attributes);
                result.InnerHtml = container.InnerHtml;

                var innerToggle = new TagBuilder("div");
                innerToggle.MergeAttributes(toggle.Attributes);
                innerToggle.InnerHtml = enabledRenderer?.Render(value?["isEnabled"]) + toggle.InnerHtml;

                if (settingsRenderer != null || propertiesRenderer != null)
                {
                    var sb = new StringBuilder();
                    if (propertiesRenderer != null)
                    {
                        sb.Append(propertiesRenderer.Render(value[FeatureEditor.SettingsKey]));
                    }

                    if (settingsRenderer != null)
                    {
                        if (key == FeatureEditor.OptionsKey)
                        {
                            sb.Append(settingsRenderer.Render(value[key]));
                        }
                        else
                        {
                            sb.Append(settingsRenderer.Render(v[key]));
                        }
                    }

                    innerToggle = _featureJsonEditorDecorator.DecorateSettings(Context, innerToggle, sb.ToString(),
                        title: modalTitle,
                        helpTitle: childHelpTitle,
                        helpContent: childHelpContent);
                }

                result.InnerHtml += innerToggle.ToString();

                result = Context.Decorator.DecorateItemContainer(Context, result);

                if (propertyTagRenderes.Any())
                {
                    var propertyContainer = new TagBuilder("div");
                    propertyContainer.AddCssClass("row");

                    foreach (var property in propertyTagRenderes)
                        propertyContainer.InnerHtml += property.Value.Render(v);

                    result.InnerHtml += propertyContainer.ToString();
                }
                return result.ToString();
            }, v =>
            {
                var value = v[Context.Options.Key];
                var sb = new StringBuilder();
                if (enabledRenderer != null)
                    sb.Append(enabledRenderer.JavaScript(value?["isEnabled"]));

                if (propertiesRenderer != null)
                    sb.Append(propertiesRenderer.JavaScript(value));

                if (settingsRenderer != null)
                {
                    if (key == FeatureEditor.OptionsKey)
                    {
                        sb.Append(settingsRenderer.JavaScript(value[key]));
                    }
                    else
                    {
                        sb.Append(settingsRenderer.JavaScript(v[key]));
                    }
                }

                foreach (var renderer in propertyTagRenderes)
                {
                    sb.Append(renderer.Value.JavaScript(v));
                }

                sb.Append(javaScript.Replace("{0}", value?["isEnabled"]?.ToString()?.ToLower()));

                return sb.ToString();
            });
        }

        private JsonEditorRenderer GetPropertyTagBuilder(string key, JSchema schema)
        {
            var editor = _editorProvider.GetJsonEditor(schema, $"{Context.Options.Key}.{key}", Context.Prefix.Split('.')[0], Context);

            if (editor.Context.Options.Hidden)
                return null;

            var builder = editor?.Build();
            return builder;
        }

        private JsonEditorRenderer GetChildFeatureTagBuilder(string key, JSchema schema)
        {
            var editor = _editorProvider.GetJsonEditor(schema, key, Context.Prefix.Split('.')[0], Context);

            if (editor.Context.Options.Hidden)
                return null;

            var builder = editor?.Build();
            return builder;
        }

        private JsonEditorRenderer GetSettingsTagBuilder(string key, JSchema schema)
        {
            var editor = _editorProvider.GetJsonEditor(schema, $"{Context.Options.Key}.{key}", Context.Prefix.Split('.')[0], Context);

            if (editor.Context.Options.Hidden)
                return null;

            var builder = editor?.Build();
            return builder;
        }

        private JsonEditorRenderer GetFeatureOptionsTagBuilder(string key, JSchema schema)
        {
            var editor = _editorProvider.GetJsonEditor(schema, key, Context.Prefix.Split('.')[0], Context);

            if (editor.Context.Options.Hidden)
                return null;

            var builder = editor?.Build();
            return builder;
        }
    }

    public class OptionsObjectJsonEditor : JsonEditor
    {
        private readonly IJsonEditorProvider _editorProvider;
        public OptionsObjectJsonEditor(IJsonEditorResolutionContext context, IJsonEditorProvider editorProvider, EditorOptions options = null) : base(context, options)
        {
            _editorProvider = editorProvider;
        }

        public override JsonEditorRenderer Build()
        {
            var container = new TagBuilder("div");
            container.Attributes.Add("data-editor-type", this.ToString());

            container = Context.Decorator.DecorateItemContainer(Context, container);

            var propertyTagRenderes = Context.Schema.Properties
                .Select(property => new { Key = property.Key, Value = GetPropertyTagBuilder(property.Key, property.Value) })
                .Where(x => x.Value != null)
                .ToArray();

            return new JsonEditorRenderer(Context.Serializer, value =>
            {
                var result = new TagBuilder("div");
                result.MergeAttributes(container.Attributes);
                result.InnerHtml = container.InnerHtml;

                foreach (var property in propertyTagRenderes)
                    result.InnerHtml += property.Value.Render(value?[property.Key]);

                return result.ToString();
            }, value => string.Join("", propertyTagRenderes.Select(property => property.Value.JavaScript(value?[property.Key]))));
        }

        private JsonEditorRenderer GetPropertyTagBuilder(string key, JSchema schema)
        {
            var editor = _editorProvider.GetJsonEditor(schema, key, Context.Path, Context);

            if (editor.Context.Options.Hidden)
                return null;

            var builder = editor?.Build();
            return builder;
        }
    }
}
