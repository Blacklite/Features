using Blacklite.Framework.Features.Describers;
using System;
using System.Collections.Concurrent;

namespace Blacklite.Framework.Features.Observables
{
    public class FeatureSubjectFactory : IFeatureSubjectFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<Type, IFeatureSubject> _subjects = new ConcurrentDictionary<Type, IFeatureSubject>();

        public FeatureSubjectFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFeatureSubject GetSubject(Type featureType)
        {
            return _subjects.GetOrAdd(featureType, CreateSubject);
        }

        private IFeatureSubject CreateSubject(Type featureType)
        {
            var subjectType = typeof(FeatureSubject<>).MakeGenericType(featureType);
            return (IFeatureSubject)_serviceProvider.GetService(subjectType);
        }
    }
}
