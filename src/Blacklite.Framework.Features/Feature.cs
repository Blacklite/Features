using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public interface IFeature
    {
        bool IsEnabled { get; set; }
    }

    public abstract partial class Feature : IFeature
    {
        private bool _enabled;
        public virtual bool IsEnabled
        {
            get { return _enabled; }
            set { _enabled = true; }
        }
    }
}
