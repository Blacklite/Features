using Microsoft.AspNet.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Blacklite.Framework.Features.Editors
{
    public static class BindFeatures
    {
        public static async Task<bool> LoadFormData(HttpContext httpContext, IFeatureEditor editor)
        {
            var model = editor.JToken;

            var form = await httpContext.Request.ReadFormAsync();

            foreach (var item in form)
            {
                if (item.Key.StartsWith(editor.Prefix, StringComparison.OrdinalIgnoreCase))
                {
                    var formValue = item.Value.Last();
                    var key = item.Key.Substring(editor.Prefix.Length + 1);
                    var editorModel = editor.Models.FirstOrDefault(x => {
                        return x.Name.Equals(key, StringComparison.OrdinalIgnoreCase);
                    });

                    var jValue = model[editorModel.Name];
                    if (!editorModel.Describer.IsReadOnly && jValue?.ToString() != formValue)
                    {
                        switch (jValue.Type)
                        {
                            case JTokenType.Integer:
                                int @int;
                                if (int.TryParse(formValue, out @int))
                                {
                                    model[editorModel.Name] = new JValue(@int);
                                }
                                break;
                            case JTokenType.Float:
                                float @float;
                                if (float.TryParse(formValue, out @float))
                                {
                                    model[editorModel.Name] = new JValue(@float);
                                }
                                break;
                            case JTokenType.Boolean:
                                bool @bool;
                                if (bool.TryParse(formValue, out @bool))
                                {
                                    model[editorModel.Name] = new JValue(@bool);
                                }
                                break;
                            case JTokenType.Null:
                            case JTokenType.Undefined:
                                if (int.TryParse(formValue, out @int))
                                {
                                    model[editorModel.Name] = new JValue(@int);
                                }
                                else if (float.TryParse(formValue, out @float))
                                {
                                    model[editorModel.Name] = new JValue(@float);
                                }
                                else if (bool.TryParse(formValue, out @bool))
                                {
                                    model[editorModel.Name] = new JValue(@bool);
                                }
                                else
                                {
                                    model[editorModel.Name] = new JValue(formValue);
                                }
                                break;
                            case JTokenType.String:
                            case JTokenType.Uri:
                            case JTokenType.Guid:
                            default:
                                model[editorModel.Name] = new JValue(formValue);
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
