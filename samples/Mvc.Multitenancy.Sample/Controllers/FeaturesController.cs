using System;
using System.Collections;
using System.Collections.Generic;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Editors;
using Blacklite.Framework.Features.Mvc;
using Microsoft.AspNet.Mvc;
using System.Linq;
using Blacklite.Framework.Multitenancy.Features.Describers;
using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Editors.Factory;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Mvc.Sample.Controllers
{
    public class FeatureDescribers : FeatureDescriberCollection
    {
        public FeatureDescribers(IEnumerable<IFeatureDescriber> describers)
            : base(describers
                  .OfType<MultitenancyFeatureDescriber>()
                  .Where(z => !z.IsTenantScoped))
        { }
    }
    public class TenantFeatureDescribers : FeatureDescriberCollection
    {
        public TenantFeatureDescribers(IEnumerable<IFeatureDescriber> describers)
            : base(describers
                  .OfType<MultitenancyFeatureDescriber>()
                  .Where(z => z.IsTenantScoped))
        { }
    }

    public class FeaturesController : FeatureController<FeatureDescribers> { }

    public class TenantFeaturesController : FeatureController<TenantFeatureDescribers> { }
}
