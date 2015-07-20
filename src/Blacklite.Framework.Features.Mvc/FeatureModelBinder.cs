using Blacklite.Framework.Features.Editors;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.OptionsModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Framework.DependencyInjection;
using System.Reflection;
using Newtonsoft.Json.Schema;

namespace Blacklite.Framework.Features.Mvc
{
    public class FeatureModelBinder : IModelBinder
    {
        public async Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!typeof(IFeatureEditor).GetTypeInfo().IsAssignableFrom(bindingContext.ModelType.GetTypeInfo()))
            {
                return new ModelBindingResult(null, null, false);
            }

            var request = bindingContext.OperationBindingContext.HttpContext.Request;
            if (!request.HasFormContentType)
            {
                return new ModelBindingResult(null, null, false);
            }

            var editor = (IFeatureEditor)bindingContext.OperationBindingContext.HttpContext.RequestServices.GetService(bindingContext.ModelType);

            var model = editor.JToken;
            //var jsonEditorProvider = bindingContext.OperationBindingContext.HttpContext.RequestServices.GetService<IJsonEditorProvider>();

            var result = await BindFeatures.LoadFormData(bindingContext.OperationBindingContext.HttpContext, editor);

            bindingContext.Model = editor;

            return new ModelBindingResult(editor, "Model", true);
        }
    }
}
