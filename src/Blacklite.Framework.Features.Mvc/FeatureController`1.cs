using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Editors;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Mvc;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Blacklite.Framework.Features.Mvc
{
    public class FeatureController<TCollection> : Controller
        where TCollection : FeatureDescriberCollection
    {
        protected readonly IFileProvider FileProvider;
        public FeatureController()
        {
            FileProvider = new EmbeddedFileProvider(typeof(FeatureController<>).GetTypeInfo().Assembly);
        }

        [HttpGet]
        public virtual IActionResult Index([FromServices] IFeatureEditor<FeatureEditorFactory<TCollection>> editor)
        {
            return View(editor);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Index")]
        public virtual IActionResult IndexPost([ModelBinder(BinderType = typeof(FeatureModelBinder))] IFeatureEditor<FeatureEditorFactory<TCollection>> editor)
        {
            if (this.TryValidateModel(editor))
            {
                editor.Save();
            }
            return View(editor);
        }

        public virtual string Schema([FromServices] IFeatureEditor<FeatureEditorFactory<TCollection>> editor)
        {
            return editor.Schema.ToString();
        }

        public virtual string Json([FromServices] IFeatureEditor<FeatureEditorFactory<TCollection>> editor)
        {
            return editor.Model.ToString();
        }

        private string _resources;

        public async virtual Task<IActionResult> Scripts()
        {
            /*
                <script src="~/lib/bootstrap-switch/js/bootstrap-switch.min.js"></script>
            */
            if (_resources == null)
            {
                using (var stream = FileProvider.GetFileInfo("compiler/resources/lib/bootstrap-switch/js/bootstrap-switch.min.js").CreateReadStream())
                using (var reader = new StreamReader(stream))
                {
                    _resources = await reader.ReadToEndAsync();
                }
            }

            return Content(_resources, "text/javascript");
        }

        public async virtual Task<IActionResult> Styles()
        {
            /*
                <link rel="stylesheet" href="~/lib/bootstrap-switch/css/bootstrap-switch.min.css" />
            */
            if (_resources == null)
            {
                using (var stream = FileProvider.GetFileInfo("compiler/resources/lib/bootstrap-switch/css/bootstrap-switch.min.css").CreateReadStream())
                using (var reader = new StreamReader(stream))
                {
                    _resources = await reader.ReadToEndAsync();
                }
            }

            return Content(_resources, "text/css");
        }

    }
}
