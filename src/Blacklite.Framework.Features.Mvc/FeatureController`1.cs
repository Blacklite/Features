using Blacklite.Framework.Features.Editors;
using Microsoft.AspNet.Mvc;
using System;

namespace Blacklite.Framework.Features.Mvc
{
    public class FeatureController<TFactory> : Controller
        where TFactory : IFeatureEditorFactory
    {
        [HttpGet]
        public virtual IActionResult Index([FromServices] IFeatureEditor<TFactory> editor)
        {
            return View(editor);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Index")]
        public virtual IActionResult IndexPost([ModelBinder(BinderType = typeof(FeatureModelBinder))] IFeatureEditor<TFactory> editor)
        {
            if (this.TryValidateModel(editor))
            {
                editor.Save();
            }
            return View(editor);
        }

        public virtual string Schema([FromServices] IFeatureEditor<TFactory> editor)
        {
            return editor.Schema.ToString();
        }

        public virtual string Json([FromServices] IFeatureEditor<TFactory> editor)
        {
            return editor.Model.ToString();
        }
    }
}
