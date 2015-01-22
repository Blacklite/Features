using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public interface IFeature
    {
        bool IsEnabled { get; }
    }

    public abstract partial class Feature : IFeature
    {
        private bool _enabled;
        public virtual bool IsEnabled
        {
            get { return _enabled; }
        }
    }
}
