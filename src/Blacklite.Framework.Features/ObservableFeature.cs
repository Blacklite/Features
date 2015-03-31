using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Observables;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Reactive.Subjects;

namespace Blacklite.Framework.Features
{
    public abstract class ObservableFeature : Feature, IObservableFeature { }

    class ObservableFeatureImpl<T> : ObservableFeature<T>
        where T : class, IObservableFeature, new()
    {
        private readonly IFeatureSubject<T> _feature;
        public ObservableFeatureImpl(IFeatureSubjectFactory factory)
        {
            _feature = (IFeatureSubject<T>)factory.GetSubject(typeof(T));
        }

        public T Value { get { return _feature.Value; } }

        public IDisposable Subscribe(IObserver<T> observer) => _feature.Subscribe(observer);
    }
}
