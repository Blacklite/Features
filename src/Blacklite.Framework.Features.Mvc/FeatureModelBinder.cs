using Blacklite.Framework.Features.EditorModel;
using Blacklite.Json.Schema;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.OptionDescriptors;
using Microsoft.Framework.OptionsModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using Temp.Newtonsoft.Json.Linq;
using Microsoft.Framework.DependencyInjection;
using System.Reflection;
using Temp.Newtonsoft.Json.Schema;

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
            var form = await request.ReadFormAsync();

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

            bindingContext.Model = editor;

            return true;
        }
    }
}
