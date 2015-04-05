using Blacklite.Json.Schema;
using Microsoft.AspNet.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Temp.Newtonsoft.Json.Linq;
using Temp.Newtonsoft.Json.Schema;

namespace Blacklite.Framework.Features.Editors
{
    public static class BindFeatures
    {
        public static async Task<bool> LoadFormData(HttpContext httpContext, IFeatureEditor editor, IJsonEditorProvider jsonEditorProvider)
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
