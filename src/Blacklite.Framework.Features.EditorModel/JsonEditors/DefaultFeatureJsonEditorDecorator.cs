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
    public interface IFeatureJsonEditorDecorator
    {
        bool HasChildProperties(IJsonEditorResolutionContext context);
        TagBuilder DecorateFeatureCheckbox(IJsonEditorResolutionContext context, TagBuilder tagBuilder);
        TagBuilder DecorateSettings(IJsonEditorResolutionContext context, TagBuilder tagBuilder, JsonEditorRenderer renderer, object value);
        TagBuilder DecorateTabHeaderContainer(IJsonEditorResolutionContext context, TagBuilder container, IEnumerable<TagBuilder> tabs);
        TagBuilder DecorateTabHeader(IJsonEditorResolutionContext context, JSchema schema, TagBuilder container);
        TagBuilder DecorateTabContainer(IJsonEditorResolutionContext context, TagBuilder container, IEnumerable<TagBuilder> tabs);
        TagBuilder DecorateTab(IJsonEditorResolutionContext context, JSchema schema, TagBuilder container);
    }

    public class DefaultFeatureJsonEditorDecorator : IFeatureJsonEditorDecorator
    {
        private readonly Lazy<IJsonEditorProvider> _editorProvider;
        public DefaultFeatureJsonEditorDecorator(IServiceProvider serviceProvider)
        {
            _editorProvider = new Lazy<IJsonEditorProvider>(() => (IJsonEditorProvider)serviceProvider.GetService(typeof(IJsonEditorProvider)));
        }

        public virtual TagBuilder DecorateFeatureCheckbox(IJsonEditorResolutionContext context, TagBuilder toggle)
        {
            toggle.AddCssClass("feature");
            toggle.AddCssClass("col-sm-3");
            toggle.AddCssClass("col-sm-pull-0");
            toggle.AddCssClass("col-xs-5");
            toggle.AddCssClass("col-xs-pull-7");

            if (HasChildProperties(context) || context.Schema.ExtensionData.ContainsKey("child"))
            {
                toggle.AddCssClass("col-lg-5");
                toggle.AddCssClass("col-lg-offset-1");
                toggle.AddCssClass("col-md-4");
            }
            else
            {
                toggle.AddCssClass("col-lg-2");
                toggle.AddCssClass("col-lg-offset-1");
                toggle.AddCssClass("col-md-2");
            }

            return toggle;
        }

        public TagBuilder DecorateSettings(IJsonEditorResolutionContext context, TagBuilder settings, JsonEditorRenderer renderer, object value)
        {
            var container = new TagBuilder("div");
            container.AddCssClass("pull-right-sm");
            container.AddCssClass("pull-left-xs");

            var id = TagBuilder.CreateSanitizedId($"{context.Path}_settings", "_");

            var button = new TagBuilder("button");
            button.AddCssClass("btn");
            button.AddCssClass("btn-primary");
            button.AddCssClass("btn-sm");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("id", id);
            button.Attributes.Add("data-toggle", "modal");
            button.Attributes.Add("data-target", $"#{id}_modal");
            button.InnerHtml = @"<span class=""glyphicon glyphicon-cog""></span>";
            container.InnerHtml += button.ToString();

            var modal = new TagBuilder("div");
            modal.AddCssClass("modal");
            modal.Attributes.Add("id", $"{id}_modal");
            modal.Attributes.Add("tabindex", "-1");
            modal.Attributes.Add("role", "dialog");
            modal.Attributes.Add("aria-lablledby", $"{id}_header");
            modal.Attributes.Add("aria-hidden", "true");

            var innerModal = new TagBuilder("div");
            innerModal.AddCssClass("modal-dialog");

            var content = new TagBuilder("div");
            content.AddCssClass("modal-content");

            // FIXME

            var header = new TagBuilder("div");
            header.AddCssClass("modal-header");

            var close = new TagBuilder("div");
            close.Attributes.Add("type", "button");
            close.AddCssClass("close");
            close.Attributes.Add("data-dismiss", "modal");
            close.Attributes.Add("aria-label", "Close");
            close.InnerHtml = @"<span aria-hidden=""true"">&times;</span>";

            header.InnerHtml += close.ToString();

            var h4 = new TagBuilder("h4");
            h4.AddCssClass("modal-title");
            h4.Attributes.Add("id", $"{id}_header");
            h4.InnerHtml = "Settings";

            header.InnerHtml += h4.ToString();

            content.InnerHtml += header.ToString();

            var body = new TagBuilder("div");
            body.AddCssClass("modal-body");
            body.InnerHtml = renderer.Render(value);
            content.InnerHtml += body.ToString();

            var footer = new TagBuilder("div");
            footer.AddCssClass("modal-footer");
            var footerButton = new TagBuilder("button");
            footerButton.Attributes.Add("type", "button");
            footerButton.AddCssClass("btn");
            footerButton.AddCssClass("btn-info");
            footerButton.Attributes.Add("data-dismiss", "modal");
            footerButton.InnerHtml = "Close";
            footer.InnerHtml = footerButton.ToString();

            content.InnerHtml += footer.ToString();
            innerModal.InnerHtml += content.ToString();
            modal.InnerHtml += innerModal.ToString();
            container.InnerHtml += modal.ToString();

            settings.InnerHtml += container.ToString();

            return settings;
        }

        public TagBuilder DecorateTab(IJsonEditorResolutionContext context, JSchema schema, TagBuilder tab)
        {
            tab.Attributes.Add("id", GetTabId(context, schema.Title));
            tab.Attributes.Add("role", "tabpanel");
            tab.AddCssClass("tab-pane");

            return tab;
        }

        private string GetTabId(IJsonEditorResolutionContext context, string title)
        {
            return TagBuilder.CreateSanitizedId($"{context.Path}_{title}".ToLower(), "_");
        }

        public TagBuilder DecorateTabContainer(IJsonEditorResolutionContext context, TagBuilder container, IEnumerable<TagBuilder> tabs)
        {
            var content = new TagBuilder("div");
            content.AddCssClass("tab-content");

            // TODO: find active tab
            tabs.First().AddCssClass("active");

            content.InnerHtml += string.Join("", tabs);
            container.InnerHtml += content.ToString();

            return container;
        }

        public TagBuilder DecorateTabHeaderContainer(IJsonEditorResolutionContext context, TagBuilder container, IEnumerable<TagBuilder> tabs)
        {
            // TODO: find active tab
            tabs.First().AddCssClass("active");

            container.InnerHtml = string.Join("", tabs.Select(x => x.ToString()));

            container = new TagBuilder("div")
            {
                InnerHtml = container.ToString()
            };

            container.Attributes.Add("role", "tabpanel");

            return container;
        }

        public bool HasChildProperties(IJsonEditorResolutionContext context)
        {
            return context.Schema.Properties
                .Where(x => x.Key != "settings" && x.Key != "enabled")
                .Where(x => Visible(x.Key, x.Value, context))
                .Any();
        }

        private bool Visible(string key, JSchema schema, IJsonEditorResolutionContext context)
        {
            var editor = _editorProvider.Value.GetJsonEditor(schema, key, context.Path);

            if (editor.Context.Options.Hidden)
                return false;

            return true;
        }

        public TagBuilder DecorateTabHeader(IJsonEditorResolutionContext context, JSchema schema, TagBuilder container)
        {
            var li = new TagBuilder("li");
            li.MergeAttributes(container.Attributes);

            var a = new TagBuilder("a");
            var id = GetTabId(context, schema.Title);
            a.Attributes.Add("href", $"#{id}");
            a.Attributes.Add("aria-controls", id);
            a.Attributes.Add("role", "tab");
            a.Attributes.Add("data-toggle", "tab");
            a.InnerHtml = schema.Title;

            li.InnerHtml = a.ToString();
            li.Attributes.Add("role", "presentation");

            return li;
        }
    }
}