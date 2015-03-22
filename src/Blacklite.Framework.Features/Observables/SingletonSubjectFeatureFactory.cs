using System;

namespace Blacklite.Framework.Features.Observables
{
    public class SingletonFeatureSubjectFactory : ISingletonFeatureSubjectFactory
    {
        private readonly IFeatureSubjectFactory _factory;
        public SingletonFeatureSubjectFactory(IServiceProvider serviceProvider)
        {
            _factory = new FeatureSubjectFactory(serviceProvider);
        }

        public IFeatureSubject GetSubject(Type featureType)
        {
            return _factory.GetSubject(featureType);
        }
    }
}
