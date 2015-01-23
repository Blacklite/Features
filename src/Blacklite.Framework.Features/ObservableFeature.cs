using System;

namespace Blacklite.Framework.Features
{
    public interface IObservableFeature : IFeature { }
    public interface IObservableFeature<T> : IObservableFeature, IObservable<T>, IDisposable where T : IFeature
    {
        T Value { get; }
    }

    public class ObservableFeature<T> : IObservableFeature<T>
        where T : IFeature
    {
        private readonly IObservable<T> _feature;
        private readonly IDisposable _subscription;
        public ObservableFeature(IObservable<T> feature)
        {
            _feature = feature;
            _subscription = _feature.Subscribe(x => Value = x);
        }

        public T Value { get; private set; }

        public bool IsEnabled { get { return Value?.IsEnabled ?? false; } set { } }

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
}
