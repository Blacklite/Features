using Blacklite.Json.Schema;
using System;
using Blacklite.Json.Schema.Editors;
using Temp.Newtonsoft.Json.Schema;
using Microsoft.AspNet.Mvc.Rendering;
using System.Linq;
using Temp.Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Blacklite.Framework.Features.EditorModel.JsonEditors.Resolvers
{
    public class TabsJsonEditorResolver : IJsonEditorResolver
    {
        private readonly Lazy<IJsonEditorProvider> _editorProvider;
        private readonly IFeatureJsonEditorDecorator _featureJsonEditorDecorator;
        public TabsJsonEditorResolver(IServiceProvider serviceProvider, IFeatureJsonEditorDecorator featureJsonEditorDecorator)
        {
            _editorProvider = new Lazy<IJsonEditorProvider>(() => (IJsonEditorProvider)serviceProvider.GetService(typeof(IJsonEditorProvider)));
            _featureJsonEditorDecorator = featureJsonEditorDecorator;
        }

        public int Priority { get; } = 1000;

        public JsonEditor GetEditor(IJsonEditorResolutionContext context)
        {
            if (context.Schema.Format == "tabs" && context.Schema.Type == JSchemaType.Object)
                return new TabsObjectJsonEditor(context, _editorProvider.Value, _featureJsonEditorDecorator);
            if (context.Schema.Format == "rows" && context.Schema.Type == JSchemaType.Object)
                return new RowsObjectJsonEditor(context, _editorProvider.Value);
            if (context.Schema.Format == "feature" && context.Schema.Type == JSchemaType.Object)
                return new FeatureObjectJsonEditor(context, _editorProvider.Value, _featureJsonEditorDecorator);
            if (context.Schema.Format == "feature-inline" && context.Schema.Type == JSchemaType.Object)
                return new FeatureInlineObjectJsonEditor(context, _editorProvider.Value);
            if (context.Schema.Format == FeatureEditor.OptionsKey && context.Schema.Type == JSchemaType.Object)
                return new OptionsObjectJsonEditor(context, _editorProvider.Value);

            return null;
        }
    }
}
