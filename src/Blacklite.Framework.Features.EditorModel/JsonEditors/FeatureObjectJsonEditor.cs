using Blacklite.Json.Schema;
using System;
using Blacklite.Json.Schema.Editors;
using Temp.Newtonsoft.Json.Schema;
using Microsoft.AspNet.Mvc.Rendering;
using System.Linq;
using Temp.Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;

namespace Blacklite.Framework.Features.EditorModel.JsonEditors
{
    public class FeatureObjectJsonEditor : JsonEditor
    {
        private readonly IJsonEditorProvider _editorProvider;
        private readonly IFeatureJsonEditorDecorator _featureJsonEditorDecorator;
        public FeatureObjectJsonEditor(IJsonEditorResolutionContext context,
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
            if (Context.Schema.Properties.ContainsKey("enabled"))
            {
                enabledRenderer = GetOptionsTagBuilder("enabled", Context.Schema.Properties["enabled"]);
            }

            var propertyTagRenderes = Context.Schema.Properties
                .Where(x => x.Key != FeatureEditor.SettingsKey && x.Key != FeatureEditor.OptionsKey && x.Key != "enabled")
                .Select(property => new { Key = property.Key, Value = GetOptionsTagBuilder(property.Key, property.Value) })
                .Where(x => x.Value != null)
                .ToArray();

            var title = new TagBuilder("div");
            title.InnerHtml = this.GetTitle();
            title = Context.Decorator.DecorateTitle(Context, title);

            container.InnerHtml += title.ToString();

            var toggle = new TagBuilder("div");
            _featureJsonEditorDecorator.DecorateFeatureCheckbox(Context, toggle);

            var modalTitle = "Settings";
            if (Context.Schema.Properties.ContainsKey(FeatureEditor.SettingsKey))
            {
                modalTitle = "Settings";
                propertiesRenderer = GetPropertyTagBuilder(FeatureEditor.SettingsKey, Context.Schema.Properties[FeatureEditor.SettingsKey]);
            }

            if (Context.Schema.Properties.ContainsKey(FeatureEditor.OptionsKey))
            {
                modalTitle = Context.Schema.Title;
                //modalTitle = Context.Schema.Properties[FeatureEditor.OptionsKey].Title;
                settingsRenderer = GetOptionsTagBuilder(FeatureEditor.OptionsKey, Context.Schema.Properties[FeatureEditor.OptionsKey]);
            }

            return new JsonEditorRenderer(Context.Serializer, value =>
            {
                var result = new TagBuilder("div");
                result.MergeAttributes(container.Attributes);
                result.InnerHtml = container.InnerHtml;

                var innerToggle = new TagBuilder("div");
                innerToggle.MergeAttributes(toggle.Attributes);
                innerToggle.InnerHtml += enabledRenderer?.Render(value?["enabled"]);

                if (settingsRenderer != null || propertiesRenderer != null)
                {
                    var sb = new StringBuilder();
                    if (propertiesRenderer != null)
                    {
                        sb.Append(propertiesRenderer.Render(value));
                    }

                    if (settingsRenderer != null)
                    {
                        sb.Append(settingsRenderer.Render(value?[FeatureEditor.OptionsKey]));
                    }

                    innerToggle = _featureJsonEditorDecorator.DecorateSettings(Context, innerToggle, sb.ToString(), modalTitle);
                }

                result.InnerHtml += innerToggle.ToString();

                result = Context.Decorator.DecorateItemContainer(Context, result);

                if (propertyTagRenderes.Any())
                {
                    var propertyContainer = new TagBuilder("div");
                    propertyContainer.AddCssClass("row");

                    foreach (var property in propertyTagRenderes)
                        propertyContainer.InnerHtml += property.Value.Render(value?[property.Key]);

                    result.InnerHtml += propertyContainer.ToString();
                }
                return result.ToString();
            });
        }

        private JsonEditorRenderer GetOptionsTagBuilder(string key, JSchema schema)
        {
            var editor = _editorProvider.GetJsonEditor(schema, key, Context.Path);

            if (editor.Context.Options.Hidden)
                return null;

            var builder = editor?.Build();
            return builder;
        }

        private JsonEditorRenderer GetPropertyTagBuilder(string key, JSchema schema)
        {
            var editor = _editorProvider.GetJsonEditor(schema, key, Context.Path);

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
            });
        }

        private JsonEditorRenderer GetPropertyTagBuilder(string key, JSchema schema)
        {
            var editor = _editorProvider.GetJsonEditor(schema, key, Context.Path);

            if (editor.Context.Options.Hidden)
                return null;

            var builder = editor?.Build();
            return builder;
        }
    }
}
