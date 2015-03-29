using System;
using Blacklite.Framework.Features.Factory;

namespace Blacklite.Framework.Features
{
    public abstract partial class Feature : IFeature { }

    public class FeatureImpl<TFeature> : Feature<TFeature>
        where TFeature : class, new()
    {
        private readonly IFeatureFactory _factory;
        private object _lock = new object();
        private Lazy<TFeature> _value;

        public FeatureImpl(IFeatureFactory factory)
        {
            _factory = factory;
            _value = new Lazy<TFeature>(Configure);
        }
        public TFeature Value { get { return _value.Value; } }

        public virtual TFeature Configure()
        {
            return _factory.GetFeature<TFeature>();
        }
    }
}
