using Blacklite.Framework.Features;
using Blacklite.Framework.Multitenancy;
using System;

namespace Mvc.Sample.Features
{
    [FeatureDisplayName("Hero Message")]
    public class HeroOptions : ObservableFeature
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }

    [FeatureDisplayName("Hero Message"), TenantOnly]
    public class HeroMessage : ObservableSwitch<HeroOptions> { }
}
