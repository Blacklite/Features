using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Builder;
using System.Threading.Tasks;
using Temp.Newtonsoft.Json.Linq;
using Blacklite.Framework.Features.Editors;
using Blacklite.Json.Schema;
using System.Linq;
using Temp.Newtonsoft.Json.Schema;
using Microsoft.AspNet.FileProviders;
using System.IO;
using Blacklite.Json.Schema.Editors;
using Microsoft.AspNet.Mvc;

namespace Blacklite.Framework.Features.Http.Extensions
{
    public class FeatureMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IFeatureEditorFactory _factory;
        private readonly IFileProvider _fileProvider;
        private readonly string _layout;
        private readonly FeatureOptions _options;

        public FeatureMiddleware(RequestDelegate next, FeatureOptions options)
        {
            _next = next;
            _factory = options.Factory;
            _fileProvider = options.FileProvider;
            _layout = options.Layout;
            _options = options;
        }

        // TODO: Property construct the AntiForgery so it doesn't require Mvc be added.
        public async Task Invoke(HttpContext httpContext, IJsonEditorProvider jsonEditorProvider, AntiForgery antiForgery)
        {
            var editor = _factory.GetFeatureEditor();

            if (httpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                await antiForgery.ValidateAsync(httpContext);
                var result = await LoadFormData(httpContext, editor, jsonEditorProvider);

                if (result)
                {
                    editor.Save();
                }
            }

            var renderer = jsonEditorProvider.GetJsonEditor(editor.Schema, editor.Prefix, _options.JsonEditorResolver).Build();

            await httpContext.Response.WriteAsync(_layout
                .Replace("{{path}}", _options.Path)
                .Replace("{{title}}", _options.Title)
                .Replace("{{content}}", GetContent(renderer, editor))
                .Replace("{{scripts}}", GetScripts(renderer, editor))
                .Replace("{{antiforgery}}", antiForgery.GetHtml(httpContext).ToString())
            );
        }

        private string GetContent(JsonEditorRenderer renderer, IFeatureEditor editor)
        {
            return renderer.Render(editor.Model);
        }

        private string GetScripts(JsonEditorRenderer renderer, IFeatureEditor editor)
        {
            var scripts = renderer.JavaScript(editor.Model);

            return $@"<script>$(function() {{
    var elements = {{}};
    var callbacks = [];

    {scripts}

    callbacks.forEach(function(x) {{ x(); }});
}});
</script>";
        }

        private async Task<bool> LoadFormData(HttpContext httpContext, IFeatureEditor editor, IJsonEditorProvider jsonEditorProvider)
        {
            var model = editor.Model;

            var form = await httpContext.Request.ReadFormAsync();

            foreach (var item in form)
            {
                if (item.Key.StartsWith(editor.Prefix, StringComparison.OrdinalIgnoreCase))
                {
                    // If they have duplicate values, take the last one put in
                    // This helps solve the checkbox case (hidden = false, checked = true)
                    var formValue = item.Value.Last();


                    var definitions = (JObject)editor.Schema.ExtensionData["definitions"];
                    var keys = item.Key.Substring(editor.Prefix.Length + 1).Split('.');
                    var schema = (JSchema)definitions[keys[0]];
                    var parent = model[keys[0]];
                    keys = keys.Skip(1).ToArray();

                    var valueKey = keys.Last();

                    foreach (var key in keys.Take(keys.Length - 1))
                    {
                        parent = parent?[key];
                        schema = schema.Properties?[key];
                    }

                    schema = schema.Properties?[valueKey];
                    var resolutionContext = jsonEditorProvider.GetResolutionContext(schema, editor.Prefix);

                    var value = parent[valueKey];

                    if (!resolutionContext.Options.ReadOnly && value?.ToString() != formValue)
                    {
                        switch (value.Type)
                        {
                            case JTokenType.Integer:
                                int @int;
                                if (int.TryParse(formValue, out @int))
                                {
                                    parent[valueKey] = new JValue(@int);
                                }
                                break;
                            case JTokenType.Float:
                                float @float;
                                if (float.TryParse(formValue, out @float))
                                {
                                    parent[valueKey] = new JValue(@float);
                                }
                                break;
                            case JTokenType.Boolean:
                                bool @bool;
                                if (bool.TryParse(formValue, out @bool))
                                {
                                    parent[valueKey] = new JValue(@bool);
                                }
                                break;
                            case JTokenType.Null:
                            case JTokenType.Undefined:
                                if (int.TryParse(formValue, out @int))
                                {
                                    parent[valueKey] = new JValue(@int);
                                }
                                else if (float.TryParse(formValue, out @float))
                                {
                                    parent[valueKey] = new JValue(@float);
                                }
                                else if (bool.TryParse(formValue, out @bool))
                                {
                                    parent[valueKey] = new JValue(@bool);
                                }
                                else
                                {
                                    parent[valueKey] = new JValue(formValue);
                                }
                                break;
                            case JTokenType.String:
                            case JTokenType.Uri:
                            case JTokenType.Guid:
                            default:
                                parent[valueKey] = new JValue(formValue);
                                break;
                                //case JTokenType.Date:
                                //    break;
                                //case JTokenType.TimeSpan:
                                //    break;
                        }
                    }
                }
            }

            return true;
        }
    }
}
