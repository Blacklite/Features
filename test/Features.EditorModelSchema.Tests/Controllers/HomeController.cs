using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Blacklite.Framework.Features.EditorModel;
using Blacklite.Framework.Features;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Threading.Tasks;
using Blacklite.Framework;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Core.Collections;
using Temp.Newtonsoft.Json.Linq;
using System.ComponentModel;
using Blacklite.Json.Schema;

namespace Features.EditorModelSchema.Tests.Controllers
{
    public class FeatureModelBinder<TFactory> : IModelBinder
        where TFactory : IFeatureEditorFactory
    {
        private readonly IFeatureEditor _editor;
        private readonly IJsonEditorProvider _jsonEditorProvider;
        public FeatureModelBinder(TFactory factory, IJsonEditorProvider jsonEditorProvider)
        {
            _editor = factory.GetFeatureEditor();
            _jsonEditorProvider = jsonEditorProvider;
        }
        /// <inheritdoc />
        public async Task<bool> BindModelAsync([NotNull] ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(IFeatureEditor))
            {
                return false;
            }

            var model = _editor.Model;

            var request = bindingContext.OperationBindingContext.HttpContext.Request;
            if (request.HasFormContentType)
            {
                var form = request.ReadFormAsync();
                form.Wait();

                foreach (var item in form.Result)
                {
                    if (item.Key.StartsWith(_editor.Prefix))
                    {
                        var formValue = item.Value[0];

                        var parent = model;
                        var schema = _editor.Schema;
                        var keys = item.Key.Substring(_editor.Prefix.Length + 1).Split('.');
                        var valueKey = keys.Last();

                        foreach (var key in keys.Take(keys.Length - 1))
                        {
                            parent = parent?[key];
                            schema = schema.Properties?[key];
                        }

                        schema = schema.Properties?[valueKey];
                        var resolutionContext = _jsonEditorProvider.GetResolutionContext(schema, _editor.Prefix);

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
            }

            bindingContext.Model = _editor;

            return true;
        }
    }

    public class HomeController : Controller
    {
        [HttpGet, HttpPost]
        public IActionResult Index([ModelBinder(BinderType = typeof(FeatureModelBinder<IFeatureEditorFactory>))] IFeatureEditor editor)
        {
            if (Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                editor.Save(ModelState);
            }
            return View(editor);
        }

        //[HttpPost]
        //public IActionResult Index( [ModelBinder(BinderType =typeof(FormCollectionModelBinder))] JObject editor) // todo model binder!
        //{
        //    return View(_editor);
        //}

        public string Schema([ModelBinder(BinderType = typeof(FeatureModelBinder<IFeatureEditorFactory>))] IFeatureEditor editor)
        {
            return editor.Schema.ToString();
        }

        public string Json([ModelBinder(BinderType = typeof(FeatureModelBinder<IFeatureEditorFactory>))] IFeatureEditor editor)
        {
            return editor.Model.ToString();
        }

        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
