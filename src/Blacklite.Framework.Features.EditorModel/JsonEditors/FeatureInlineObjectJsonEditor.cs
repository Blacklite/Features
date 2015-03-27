using Blacklite.Json.Schema;
using Blacklite.Json.Schema.Editors;
using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Temp.Newtonsoft.Json.Linq;
using Temp.Newtonsoft.Json.Schema;

namespace Blacklite.Framework.Features.EditorModel.JsonEditors
{
    public class FeatureInlineObjectJsonEditor : JsonEditor
    {
        private readonly IJsonEditorProvider _editorProvider;
        private readonly IFeatureJsonEditorDecorator _featureJsonEditorDecorator;
        public FeatureInlineObjectJsonEditor(IJsonEditorResolutionContext context, IJsonEditorProvider editorProvider, IFeatureJsonEditorDecorator featureJsonEditorDecorator, EditorOptions options = null) : base(context, options)
        {
            _editorProvider = editorProvider;
            _featureJsonEditorDecorator = featureJsonEditorDecorator;
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

            string javaScript = string.Empty;
            var requires = Context.Schema.ExtensionData["requires"] as IDictionary<string, JToken>;
            if (requires != null)
            {
                var elementId = TagBuilder.CreateSanitizedId($"{Context.Prefix.Split('.')[0]}.{Context.Options.Key}.isEnabled", "_");
                var requireObjects = requires.Select(item => new { id = TagBuilder.CreateSanitizedId($"{Context.Prefix.Split('.')[0]}.{item.Key}", "_"), value = item.Value["requiredValue"].ToString().ToLowerInvariant() });
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

            //Context.Schema.ExtensionData

            return new JsonEditorRenderer(Context.Serializer, value =>
            {
                var result = new TagBuilder("div");
                result.MergeAttributes(container.Attributes);
                result.InnerHtml = container.InnerHtml;

                foreach (var property in propertyTagRenderes)
                    result.InnerHtml += property.Value.Render(value?[property.Key]);

                return result.ToString();
            }, value =>
            {
                var sb = new StringBuilder();
                foreach (var property in propertyTagRenderes)
                {
                    sb.Append(property.Value.JavaScript(value?[property.Key]));
                }

                sb.Append(javaScript.Replace("{0}", value?["isEnabled"]?.ToString()?.ToLower()));

                return sb.ToString();
            });
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
