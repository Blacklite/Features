using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Temp.Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Temp.Newtonsoft.Json.Schema;
using Microsoft.AspNet.Mvc;
using Blacklite.Framework.Features.EditorModel;

namespace Blacklite.Json.Schema.TagHelpers
{
    public class FeatureEditorTagHelper : TagHelper
    {
        [Required]
        public IFeatureEditor Editor { get; set; }

        [Activate]
        public IJsonEditorProvider EditorProvider { get; private set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Content = EditorProvider.GetJsonEditor(Editor.Schema, Editor.Prefix).Build().Render(Editor.Model);
        }
    }
}
