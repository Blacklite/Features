using Blacklite.Framework.Features.Editors;
using Blacklite.Json.Schema;
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
        public async Task<bool> BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!typeof(IFeatureEditor).GetTypeInfo().IsAssignableFrom(bindingContext.ModelType.GetTypeInfo()))
            {
                return false;
            }

            var request = bindingContext.OperationBindingContext.HttpContext.Request;
            if (!request.HasFormContentType)
            {
                return false;
            }

            var editor = (IFeatureEditor)bindingContext.OperationBindingContext.HttpContext.RequestServices.GetService(bindingContext.ModelType);

            var model = editor.Model;
            var jsonEditorProvider = bindingContext.OperationBindingContext.HttpContext.RequestServices.GetService<IJsonEditorProvider>();

            var result = await BindFeatures.LoadFormData(bindingContext.OperationBindingContext.HttpContext, editor, jsonEditorProvider);

            bindingContext.Model = editor;

            return result;
        }
    }
}
