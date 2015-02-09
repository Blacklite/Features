using System;
using System.Reactive.Subjects;

namespace Blacklite.Framework.Features
{
    // Create analyzier to identify miss used IObservableFeatures
    public interface IObservableFeature : IFeature { }

    public abstract class ObservableFeature : SetableFeature, IObservableFeature
    {
        protected ObservableFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures)
        {
        }
    }

    public interface IObservableFeature<T> : IObservable<T>
        where T : IObservableFeature
    {
    }

    class ObservableFeature<T> : IObservableFeature<T>
        where T : IObservableFeature
    {
        private readonly ISubject<T> _feature;
        public ObservableFeature(ISubjectFeature<T> feature)
        {
            _feature = feature;
        }

        public IDisposable Subscribe(IObserver<T> observer) => _feature.Subscribe(observer);
    }

    interface ISubjectFeature<T> : ISubject<T> where T : IFeature
    {

    }

    class SubjectFeature<T> : ISubjectFeature<T>
        where T : IObservableFeature
    {
        private readonly ISubject<T> _feature;
        public SubjectFeature(T feature)
        {
            _feature = new BehaviorSubject<T>(feature);
        }

        public void OnNext(T value)
        {
            _feature.OnNext(value);
        }

        void IObserver<T>.OnError(Exception error)
        {
            _feature.OnError(error);
        }

        void IObserver<T>.OnCompleted()
        {
            _feature.OnCompleted();
        }

        IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
        {
            return _feature.Subscribe(observer);
        }
    }
}
