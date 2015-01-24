using System;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features
{
    public interface IValidateFeatureService : IDisposable
    {
        bool Validate();
    }

    class ValidateFeatureService : IValidateFeatureService
    {
        private readonly IEnumerable<IDisposable> _disposables;
        private readonly Func<bool> _validate;

        public ValidateFeatureService(Func<bool> validate, IEnumerable<IDisposable> disposables = null)
        {
            _disposables = disposables ?? Enumerable.Empty<IDisposable>();
            _validate = validate;
        }

        public bool Validate()
        {
            return _validate();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var disposable in _disposables)
                        disposable.Dispose();
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ValidateFeatureService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

}
