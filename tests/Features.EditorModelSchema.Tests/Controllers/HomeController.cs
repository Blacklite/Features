using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Blacklite.Framework.Features.EditorModel;
using Blacklite.Framework.Features;

namespace Features.EditorModelSchema.Tests.Controllers
{
    public class HomeController : Controller
    {
        private readonly FeatureEditor _editor;
        public HomeController(IFeatureEditorFactory factory)
        {
            _editor = factory.GetFeatureEditor();
        }

        public IActionResult Index()
        {
            return View(_editor);
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