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

    public class TabsObjectJsonEditor : JsonEditor
    {
        private readonly IJsonEditorProvider _editorProvider;
        private readonly IFeatureJsonEditorDecorator _featureJsonEditorDecorator;
        public TabsObjectJsonEditor(IJsonEditorResolutionContext context, IJsonEditorProvider editorProvider, IFeatureJsonEditorDecorator featureJsonEditorDecorator, EditorOptions options = null) : base(context, options)
        {
            _editorProvider = editorProvider;
            _featureJsonEditorDecorator = featureJsonEditorDecorator;
        }

        public override JsonEditorRenderer Build()
        {
            var container = new TagBuilder("ul");
            container.Attributes.Add("data-editor-type", this.ToString());

            var tabs = Context.Schema.Properties
                .Select(x => _featureJsonEditorDecorator.DecorateTabHeader(Context, x.Value, new TagBuilder("li")))
                .Where(x => x != null)
                .ToArray();

            container = Context.Decorator.DecorateItemContainer(Context, container);
            container = _featureJsonEditorDecorator.DecorateTabHeaderContainer(Context, container, tabs);

            var propertyTagRenderes = Context.Schema.Properties
                .Select(property => new { Key = property.Key, Schema = property.Value, Value = GetPropertyTagBuilder(property.Key, property.Value) })
                .Where(x => x.Value != null)
                .ToArray();

            return new JsonEditorRenderer(Context.Serializer, value =>
            {
                var result = new TagBuilder("div");
                result.MergeAttributes(container.Attributes);
                result.InnerHtml = container.InnerHtml;

                var tabBuilders = new List<TagBuilder>();
                foreach (var property in propertyTagRenderes)
                {
                    var tab = new TagBuilder("div");
                    tab.InnerHtml += property.Value.Render(value);
                    tab = _featureJsonEditorDecorator.DecorateTab(Context, property.Schema, tab);
                    tabBuilders.Add(tab);
                }

                result = _featureJsonEditorDecorator.DecorateTabContainer(Context, result, tabBuilders);

                return result.ToString();
            }, value => string.Join("", propertyTagRenderes.Select(property => property.Value.JavaScript(value))));
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
