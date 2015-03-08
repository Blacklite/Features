using System;
using System.Reactive.Subjects;

namespace Blacklite.Framework.Features
{
    // Create analyzier to identify miss used IObservableFeatures
    public interface IObservableAspect : IAspect { }
    public interface IObservableFeature : IObservableAspect, IFeature { }

    public abstract class ObservableAspect : Aspect, IObservableAspect { }
    public abstract class ObservableFeature : Feature, IObservableFeature
    {
        public ObservableFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures)
        {
        }
    }

    public interface IObservableAspect<T> : IObservable<T>
        where T : IObservableAspect
    {
    }

    class ObservableAspect<T> : IObservableAspect<T>
        where T : IObservableAspect
    {
        private readonly ISubject<T> _feature;
        public ObservableAspect(ISubjectAspect<T> feature)
        {
            _feature = feature;
        }

        public IDisposable Subscribe(IObserver<T> observer) => _feature.Subscribe(observer);
    }

    interface ISubjectAspect<T> : ISubject<T>
        where T : IObservableAspect
    {

    }

    class SubjectAspect<T> : ISubjectAspect<T>
        where T : IObservableAspect
    {
        private readonly ISubject<T> _feature;
        public SubjectAspect(T feature)
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
