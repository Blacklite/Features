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

            if (httpContext.Request.Path.Value.EndsWith("/model"))
            {
                await httpContext.Response.WriteAsync(
                    Temp.Newtonsoft.Json.JsonConvert.SerializeObject(editor.Model, Temp.Newtonsoft.Json.Formatting.Indented)
                );
            }
            else if (httpContext.Request.Path.Value.EndsWith("/schema"))
            {
                await httpContext.Response.WriteAsync(
                    Temp.Newtonsoft.Json.JsonConvert.SerializeObject(editor.Schema, Temp.Newtonsoft.Json.Formatting.Indented)
                );
            }
            else
            {
                if (httpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    await antiForgery.ValidateAsync(httpContext);
                    var result = await BindFeatures.LoadFormData(httpContext, editor, jsonEditorProvider);

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
    }
}
