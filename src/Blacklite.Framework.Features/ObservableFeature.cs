using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Observables;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Reactive.Subjects;

namespace Blacklite.Framework.Features
{
    public abstract class ObservableFeature : Feature, IObservableFeature { }

    class ObservableFeatureImpl<T> : ObservableFeature<T>
        where T : class, IObservableFeature
    {
        private readonly ISubject<T> _feature;
        public ObservableFeatureImpl(IFeatureSubjectFactory factory)
        {
            _feature = (ISubject<T>)factory.GetSubject(typeof(T));
        }

        public IDisposable Subscribe(IObserver<T> observer) => _feature.Subscribe(observer);
    }
}
