using Blacklite.Framework.Features;
using System;

namespace Mvc.Sample.Features
{
    [FeatureDisplayName("Hero Message")]
    public class HeroMessage : ISwitch
    {
        public string Title { get; set; }
        public string Text { get; set; }

        public bool IsEnabled { get; set; } = false;
    }
}
