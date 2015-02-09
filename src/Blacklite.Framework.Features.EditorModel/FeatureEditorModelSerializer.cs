using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.EditorModel
{
    //public class FeatureEditorModelSchema
    //{

    //}

    //public class FeatureEditorModelSerializer
    //{
    //    public FeatureEditorModelSerializer()
    //    {

    //    }

    //    public JsonSchema Serialize(FeatureEditor editor)
    //    {
    //        var schema = new JsonSchema();
    //        schema.Title = "Features";
    //        schema.Type = JsonSchemaType.Object;
    //        schema.Properties = new Dictionary<string, JsonSchema>();
    //        foreach (var model in editor.GetFeatureModels())
    //        {
    //            schema.Properties.Add(model.DisplayName, GetModel(model));
    //        }

    //        return schema;
    //    }
        
    //    private JsonSchema GetModel(FeatureModel model)
    //    {
    //        var schema = new JsonSchema();
    //        schema.Type = JsonSchemaType.Object;
    //        schema.Title = model.Name;
    //        schema.Description = model.Description;
    //        schema.Properties = new Dictionary<string, JsonSchema>();

    //        schema.Properties.Add("Enabled", GetProperty(model.Properties.First()));

    //        var options = new JsonSchema();
    //        options.Properties = new Dictionary<string, JsonSchema>();
    //        options.Title = "Options";
    //        options.Type = JsonSchemaType.Object;
    //        foreach (var property in model.Properties.Skip(1))
    //        {
    //            options.Properties.Add(property.Name, GetProperty(property));
    //        }

    //        schema.Properties.Add("Options", options);

    //        foreach (var child in model.Children)
    //        {
    //            schema.Properties.Add(child.Name, GetModel(child));
    //        }

    //        return schema;
    //    }

    //    private JsonSchema GetProperty(FeatureOptionPropertyModel property)
    //    {
    //        // change this
    //        var schema = new JsonSchemaGenerator().Generate(property.Type);
    //        schema.Description = property.Description;
    //        schema.ReadOnly = property.IsReadOnly;
    //        return schema;
    //    }
    //}
}