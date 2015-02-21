using Blacklite.Json.Schema;
using System;
using Blacklite.Json.Schema.Editors;
using Temp.Newtonsoft.Json.Schema;
using Microsoft.AspNet.Mvc.Rendering;
using System.Linq;
using Temp.Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
            JsonEditorRenderer settingsRenderer = null;
            if (Context.Schema.Properties.ContainsKey("enabled"))
            {
                enabledRenderer = GetPropertyTagBuilder("enabled", Context.Schema.Properties["enabled"]);
            }

            var propertyTagRenderes = Context.Schema.Properties
                .Select(property => new { Key = property.Key, Value = GetPropertyTagBuilder(property.Key, property.Value) })
                .Where(x => x.Key != "settings" && x.Key != "enabled")
                .Where(x => x.Value != null)
                .ToArray();

            var title = new TagBuilder("div");
            title.InnerHtml = this.GetTitle();
            title = Context.Decorator.DecorateTitle(Context, title);

            container.InnerHtml += title.ToString();

            var toggle = new TagBuilder("div");
            _featureJsonEditorDecorator.DecorateFeatureCheckbox(Context, toggle);

            if (Context.Schema.Properties.ContainsKey("settings"))
            {
                settingsRenderer = GetPropertyTagBuilder("settings", Context.Schema.Properties["settings"]);
            }

            return new JsonEditorRenderer(Context.Serializer, value =>
            {
                var result = new TagBuilder("div");
                result.MergeAttributes(container.Attributes);
                result.InnerHtml = container.InnerHtml;

                var innerToggle = new TagBuilder("div");
                innerToggle.MergeAttributes(toggle.Attributes);
                innerToggle.InnerHtml += enabledRenderer?.Render(value?["enabled"]);

                if (settingsRenderer != null)
                {
                    innerToggle = _featureJsonEditorDecorator.DecorateSettings(Context, innerToggle, settingsRenderer, value?["settings"]);
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