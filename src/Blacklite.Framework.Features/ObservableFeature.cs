using System;
using System.Reactive.Subjects;

namespace Blacklite.Framework.Features
{
    // Create analyzier to identify miss used IObservableFeatures
    public interface IObservableFeature : IFeature { }

    public abstract class ObservableFeature : Feature, IObservableFeature
    {
        protected ObservableFeature(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures)
        {
        }
    }

    public interface IObservableFeature<T> : IObservable<T>, IDisposable
        where T : IObservableFeature
    {
    }

    class ObservableFeature<T> : IObservableFeature<T>
        where T : IObservableFeature
    {
        private readonly ISubject<T> _feature;
        private readonly IDisposable _subscription;
        private T _value;
        public ObservableFeature(ISubjectFeature<T> feature)
        {
            _feature = feature;
            _subscription = _feature.Subscribe(x => _value = x);
        }

        public bool IsEnabled { get { return _value?.IsEnabled ?? false; } set { } }

        public IDisposable Subscribe(IObserver<T> observer) => _feature.Subscribe(observer);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _subscription.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ObservableFeature() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
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

        void IObserver<T>.OnNext(T value)
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
