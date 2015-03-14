using Blacklite.Json.Schema;
using Microsoft.Framework.OptionsModel;
using System;
using Microsoft.AspNet.Mvc.Rendering;
using System.Linq;
using Temp.Newtonsoft.Json.Schema;

namespace Blacklite.Framework.Features.EditorModel.JsonEditors
{
    public class EditorModelJsonEditorDecorator : JsonEditorDecorator
    {
        private readonly Lazy<IJsonEditorProvider> _editorProvider;
        private readonly IFeatureJsonEditorDecorator _featureJsonEditorDecorator;
        public EditorModelJsonEditorDecorator(IServiceProvider serviceProvider, IFeatureJsonEditorDecorator featureJsonEditorDecorator)
        {
            _editorProvider = new Lazy<IJsonEditorProvider>(() => (IJsonEditorProvider)serviceProvider.GetService(typeof(IJsonEditorProvider)));
            _featureJsonEditorDecorator = featureJsonEditorDecorator;
        }

        public override TagBuilder DecorateContainer(IJsonEditorResolutionContext context, TagBuilder control, TagBuilder label, TagBuilder input, TagBuilder description)
        {
            if (input.TagName == "input" && input.Attributes["type"] == "checkbox")
            {
                var names = input.Attributes["name"].Split('.');
                var field = names[names.Length - 1];
                if (field.Equals("enabled", StringComparison.OrdinalIgnoreCase) && context.Schema.Format != "inline")
                {
                    control.AddCssClass("pull-right-sm");
                    control.AddCssClass("pull-left-xs");
                    label = null;
                    return base.DecorateContainer(context, control, label, input, description);
                }
            }

            if (input.TagName == "input")
            {
                control.AddCssClass("form-group");
                label.AddCssClass("control-label");
                label.AddCssClass("col-sm-3");
                label.AddCssClass("col-xs-12");
                input.AddCssClass("form-control");
                input = new TagBuilder("div")
                {
                    InnerHtml = input.ToString(TagRenderMode.SelfClosing)
                };
                input.AddCssClass("col-sm-9");
                input.AddCssClass("col-xs-12");
            }

            if (input.TagName == "textarea" || input.TagName == "select")
            {
                control.AddCssClass("form-group");
                label.AddCssClass("control-label");
                label.AddCssClass("col-sm-3");
                label.AddCssClass("col-xs-12");
                input.AddCssClass("form-control");
                input = new TagBuilder("div")
                {
                    InnerHtml = input.ToString()
                };
                input.AddCssClass("col-sm-9");
                input.AddCssClass("col-xs-12");
            }

            if (context.Schema.Format == "inline")
            {
                control = new TagBuilder(control.TagName)
                {
                    InnerHtml = base.DecorateContainer(context, control, label, input, description).ToString()
                };
                control.AddCssClass("form-horizontal");
                return control;
            }

            return base.DecorateContainer(context, control, label, input, description);
        }

        public override TagBuilder DecorateInput(IJsonEditorResolutionContext context, TagBuilder tagBuilder)
        {
            if (tagBuilder.TagName == "input" && tagBuilder.Attributes["type"] == "checkbox")
            {
                var names = tagBuilder.Attributes["name"].Split('.');
                var field = names[names.Length - 1];
                if (field.Equals("enabled", StringComparison.OrdinalIgnoreCase))
                {
                    tagBuilder.Attributes.Add("data-off-color", "danger");
                    tagBuilder.Attributes.Add("data-on-color", "info");
                    tagBuilder.Attributes.Add("data-size", "small");
                }
            }
            return base.DecorateInput(context, tagBuilder);
        }

        public override TagBuilder DecorateTitle(IJsonEditorResolutionContext context, TagBuilder title)
        {
            if (context.Schema.Format == "feature" && context.Schema.Type == JSchemaType.Object)
            {
                title = DecorateFeatureTitle(context, title);
            }
            return base.DecorateTitle(context, title);
        }

        public override TagBuilder DecorateItemContainer(IJsonEditorResolutionContext context, TagBuilder container)
        {
            if (context.Schema.Format == "feature" && context.Schema.Type == JSchemaType.Object)
            {
                if (_featureJsonEditorDecorator.HasChildProperties(context))
                {
                    container.AddCssClass("row");
                    container = new TagBuilder("div")
                    {
                        InnerHtml = container.ToString()
                    };
                    container.AddCssClass("feature");
                    container.AddCssClass("col-lg-6");
                    container.AddCssClass("col-md-6");
                    container.AddCssClass("col-sm-12");
                    container.AddCssClass("col-xs-12");

                    return container;
                }
            }

            if (context.Schema.Format == FeatureEditor.OptionsKey && context.Schema.Type == JSchemaType.Object)
            {
                container.AddCssClass("form-horizontal");
            }

            if (context.Schema.Format == "rows" && context.Schema.Type == JSchemaType.Object)
            {
                container.AddCssClass("row");
            }

            if (context.Schema.Format == "tabs" && context.Schema.Type == JSchemaType.Object)
            {
                container.AddCssClass("nav");
                container.AddCssClass("nav-tabs");
                container.Attributes.Add("role", "tablist");
            }

            return base.DecorateItemContainer(context, container);
        }

        private TagBuilder DecorateFeatureTitle(IJsonEditorResolutionContext context, TagBuilder title)
        {
            title.AddCssClass("feature");
            title.AddCssClass("feature-label");
            //title.AddCssClass("col-xs-push-3");
            title.AddCssClass("col-sm-9");
            title.AddCssClass("col-xs-7");
            title.AddCssClass("col-xs-push-5");
            title.AddCssClass("col-sm-push-0");

            if (context.Schema.ExtensionData.ContainsKey("child"))
            {
                title.AddCssClass("col-sm-push-1");
                title.AddCssClass("col-md-push-1");
                title.AddCssClass("col-lg-push-1");
            }

            if (_featureJsonEditorDecorator.HasChildProperties(context) || context.Schema.ExtensionData.ContainsKey("child"))
            {
                title.AddCssClass("col-lg-6");
                title.AddCssClass("col-md-8");
            }
            else
            {
                title.AddCssClass("col-lg-3");
                title.AddCssClass("col-md-4");
            }
            return title;
        }
    }

    public class ConfigureJsonEditorOptions : ConfigureOptions<JsonEditorOptions>
    {
        public ConfigureJsonEditorOptions(IServiceProvider serviceProvider, IFeatureJsonEditorDecorator featureJsonEditorDecorator) : base((options) => ConfigureOptions(options, serviceProvider, featureJsonEditorDecorator))
        {
        }

        public static void ConfigureOptions(JsonEditorOptions options, IServiceProvider serviceProvider, IFeatureJsonEditorDecorator featureJsonEditorDecorator)
        {
            options.Decorator = new EditorModelJsonEditorDecorator(serviceProvider, featureJsonEditorDecorator);
        }
    }
}
