using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Builder;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Blacklite.Framework.Features.Editors;
using System.Linq;
using Newtonsoft.Json.Schema;
using Microsoft.AspNet.FileProviders;
using System.IO;
using Microsoft.AspNet.Antiforgery;
using System.Text;
using Blacklite.Framework.Features.Http.Utilities;
using Blacklite.Framework.Features.Editors.Models;
using System.Collections.Generic;

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
        public async Task Invoke(HttpContext httpContext, Antiforgery antiForgery)
        {
            var editor = _factory.GetFeatureEditor();

            if (httpContext.Request.Path.Value.EndsWith("/model"))
            {
                await httpContext.Response.WriteAsync(
                    Newtonsoft.Json.JsonConvert.SerializeObject(editor.JToken, Newtonsoft.Json.Formatting.Indented)
                );
            }
            else if (httpContext.Request.Path.Value.EndsWith("/schema"))
            {
                await httpContext.Response.WriteAsync(
                    Newtonsoft.Json.JsonConvert.SerializeObject(editor.Groups, Newtonsoft.Json.Formatting.Indented)
                );
            }
            else
            {
                if (httpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    /*await antiForgery.ValidateAsync(httpContext);
                    var result = await BindFeatures.LoadFormData(httpContext, editor, jsonEditorProvider);

                    if (result)
                    {
                        editor.Save();
                    }*/
                }

                //var renderer = jsonEditorProvider.GetJsonEditor(editor.Schema, editor.Prefix, _options.JsonEditorResolver).Build();

                await httpContext.Response.WriteAsync(_layout
                    .Replace("{{path}}", _options.Path)
                    .Replace("{{title}}", _options.Title)
                    .Replace("{{header-tabs}}", GetHeaderTabs(editor))
                    .Replace("{{tab-content}}", GetHeaderTabsContent(editor))
                    .Replace("{{antiforgery}}", antiForgery.GetHtml(httpContext).ToString())
                );
            }
        }

        private string GetHeaderTabs(IFeatureEditor editor)
        {
            var sb = new StringBuilder();
            sb.Append(@"<div class=""mdl-layout__tab-bar mdl-js-ripple-effect mdl-color--primary-dark"">");

            var active = editor.Groups.First();

            foreach (var group in editor.Groups)
            {
                var innerGroup = group as Group;
                var id = HtmlId.CreateSanitizedId($"{editor.Prefix}.{group.Name}");
                sb.Append($@"<a href=""#{id}"" class=""mdl-layout__tab{(active == group ? " is-active" : "")}"">{innerGroup?.Title ?? group.Name}</a>");
            }

            sb.Append("</div>");
            return sb.ToString();
        }

        private string GetHeaderTabsContent(IFeatureEditor editor)
        {
            var sb = new StringBuilder();
            var active = editor.Groups.First();
            var enumerator = GetNextColor().GetEnumerator();
            foreach (Group group in editor.Groups)
            {
                sb.Append(GetTabs(editor, enumerator, $"{editor.Prefix}.{group.Name}", group, active == group, true));
            }
            return sb.ToString();
        }

        private string GetTabs(IFeatureEditor editor, IEnumerator<KeyValuePair<string, string>> tabClass, string id, Group group, bool isActive, bool isRoot = false)
        {
            var sb = new StringBuilder();
            sb.Append($@"<div class=""{(isRoot ? "mdl-layout__tab-panel" : "mdl-tabs__panel")}{(isActive ? " is-active" : "")}"" id=""{HtmlId.CreateSanitizedId(id)}"">");
            sb.Append($@"<div class=""mdl-tabs mdl-js-tabs mdl-js-ripple-effect"">");

            var isModels = group.Items.OfType<Model>().Any();

            var activeItem = group.Items.First();
            if (!isModels)
            {
                tabClass.MoveNext();
                sb.Append($@"<div class=""mdl-tabs__tab-bar {tabClass.Current.Key}"">");
                foreach (var item in group.Items)
                {
                    var i = HtmlId.CreateSanitizedId($"{id}.{item.Name}");
                    sb.Append($@"<a href=""#{i}"" class=""mdl-tabs__tab {tabClass.Current.Value}{(activeItem == item ? " is-active" : "")}"">{item.Name}</a>");
                }
                sb.Append(@"</div>");
                foreach (var item in group.Items)
                {
                    var i = HtmlId.CreateSanitizedId($"{id}.{item.Name}");
                    var innerGroup = item as Group;
                    if (innerGroup != null)
                    {
                        sb.Append(GetTabs(editor, tabClass, i, innerGroup, activeItem == item));
                    }
                }
            }
            else
            {
                sb.Append($@"<div class=""mdl-grid"">");
                var enumerator = GetNextColor().GetEnumerator();

                foreach (var item in group.Items)
                {
                    var i = HtmlId.CreateSanitizedId($"{id}.{item.Name}");
                    var model = item as Model;
                    if (model != null)
                    {
                        sb.Append(GetModelContent(editor, enumerator, i, model));
                    }
                }

                sb.Append(@"</div>");
            }
            sb.Append(@"</div>");
            sb.Append(@"</div>");

            return sb.ToString();
        }

        private string GetModelContent(IFeatureEditor editor, IEnumerator<KeyValuePair<string, string>> enumerator, string id, Model model)
        {
            enumerator.MoveNext();
            var background = enumerator.Current.Key;
            var text = enumerator.Current.Value;
            var sb = new StringBuilder();
            sb.Append($@"<div class=""feature mdl-cell mdl-cell--3-col-desktop mdl-cell--4-col-tablet mdl-cell--4-col-phone mdl-card mdl-shadow--2dp"">");
            if (model.HasEnabled)
            {
                sb.Append($@"
                    <div class=""mdl-card__title mdl-card--border {background} {text}"">
                        <label class=""mdl-switch mdl-js-switch mdl-js-ripple-effect"" for=""{id}"">
                            <input type=""hidden"" name=""{editor.Prefix}.{model.Name}"" id=""{id}_hidden"" value=""false"" />
                            <input type=""checkbox"" name=""{editor.Prefix}.{model.Name}"" id=""{id}"" class=""mdl-switch__input"" value=""true"" {(editor.Values[model.Name].IsEnabled.HasValue && editor.Values[model.Name].IsEnabled.Value == true ? "checked" : "")} />
                            <span class=""mdl-switch__label""></span>
                            <h2 class=""mdl-card__title-text"">{model.Title}</h2>
                        </label>
                    </div>
                    ");
            }
            else
            {
                sb.Append($@"
                        <div class=""mdl-card__title mdl-card--border {background} {text}"">
                            <h2 class=""mdl-card__title-text"">{model.Title}</h2>
                        </div>
                        ");
            }

            if (model.Description != null)
            {
                sb.Append($@"
                    <div class=""mdl-card__supporting-text"">
                    {model.Description}
                    </div>
                    ");
            }

            sb.Append($@"
                <div class=""mdl-card__actions mdl-card--border"">
                    <a class=""mdl-button mdl-button--colored mdl-js-button mdl-js-ripple-effect"">View Updates</a>
                </div>
            ");

            sb.Append($@"
            </div>
            ");

            /*
            <div class="mdl-cell mdl-cell--6-col-desktop mdl-cell--8-col-tablet mdl-cell--4-col-phone mdl-card mdl-shadow--2dp">
            	<div class="mdl-card__supporting-text">
            		<label class="mdl-checkbox mdl-js-checkbox mdl-js-ripple-effect" for="checkbox-1">
            			<input type="checkbox" id="checkbox-1" class="mdl-checkbox__input" checked />
            			<span class="mdl-checkbox__label">
                            <h2 class="mdl-card__title-text">Application Development Feature A</h2>
                        </span>
            		</label>
            	</div>
            	<div class="mdl-card__actions mdl-card--border">
            		<a class="mdl-button mdl-button--colored mdl-js-button mdl-js-ripple-effect" data-upgraded=",MaterialButton,MaterialRipple">View Updates<span class="mdl-button__ripple-container"><span class="mdl-ripple is-animating" style="width: 252.567356213853px; height: 252.567356213853px; -webkit-transform: translate(-50%, -50%) translate(95px, 9px); transform: translate(-50%, -50%) translate(95px, 9px);"></span></span></a>
            	</div>
            </div>
            */

            return sb.ToString();
        }



        private static IEnumerable<KeyValuePair<string, string>> GetNextColor()
        {
            while (true)
            {
                yield return new KeyValuePair<string, string>("mdl-color--red-500", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--purple-500", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--indigo-500", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--cyan-700", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--green-600", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--amber-900", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--pink-500", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--deep-purple-500", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--light-green-800", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--orange-800", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--blue-500", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--light-blue-600", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--teal-500", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--deep-orange-500", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--grey-600", "mdl-color-text--grey-100");
                yield return new KeyValuePair<string, string>("mdl-color--blue-grey-400", "mdl-color-text--grey-100");
            }
        }

        /*private string GetContent(JsonEditorRenderer renderer, IFeatureEditor editor)
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
        }*/

    }
}
