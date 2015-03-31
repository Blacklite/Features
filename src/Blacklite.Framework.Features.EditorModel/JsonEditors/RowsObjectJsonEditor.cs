using Blacklite.Json.Schema;
using System;
using Blacklite.Json.Schema.Editors;
using Temp.Newtonsoft.Json.Schema;
using Microsoft.AspNet.Mvc.Rendering;
using System.Linq;
using Temp.Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Blacklite.Framework.Features.Editors.JsonEditors
{

    public class RowsObjectJsonEditor : JsonEditor
    {
        private readonly IJsonEditorProvider _editorProvider;
        public RowsObjectJsonEditor(IJsonEditorResolutionContext context, IJsonEditorProvider editorProvider, EditorOptions options = null) : base(context, options)
        {
            _editorProvider = editorProvider;
        }

        public override JsonEditorRenderer Build()
        {
            var container = new TagBuilder("div");
            container.Attributes.Add("data-editor-type", this.ToString());
            container = Context.Decorator.DecorateItemContainer(Context, container);

            var propertyTagRenderes = Context.Schema.Properties
                .Select(property => new { Key = property.Key, Schema = property.Value, Value = GetPropertyTagBuilder(property.Key, property.Value) })
                .Where(x => x.Value != null)
                .ToArray();

            return new JsonEditorRenderer(Context.Serializer, value =>
            {
                var result = new TagBuilder("div");
                result.MergeAttributes(container.Attributes);
                result.InnerHtml = container.InnerHtml;

                foreach (var property in propertyTagRenderes)
                {
                    result.InnerHtml += property.Value.Render(value);
                }

                return result.ToString();
            }, value => string.Join("", propertyTagRenderes.Select(property => property.Value.JavaScript(value))));
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
