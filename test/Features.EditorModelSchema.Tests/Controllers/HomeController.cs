using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Blacklite.Framework.Features.Editors;
using Blacklite.Framework.Features;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Threading.Tasks;
using Blacklite.Framework;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Core.Collections;
using Temp.Newtonsoft.Json.Linq;
using System.ComponentModel;
using Blacklite.Json.Schema;
using Blacklite.Framework.Features.Mvc;
using Blacklite.Framework.Features.Describers;

namespace Features.EditorModelSchema.Tests.Controllers
{
    public class HomeController : FeatureController<FeatureDescriberCollection>
    {
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
