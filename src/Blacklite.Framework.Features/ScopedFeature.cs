using System;

namespace Blacklite.Framework.Features
{
    public interface IScopedFeature : IFeature
    {
    }

    public abstract partial class ScopedFeature : IScopedFeature
    {
        private bool _isEnabled;
        public virtual bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }
    }
}
