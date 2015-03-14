using System;

namespace Blacklite.Framework.Features
{
    public interface ObservableFeature<T> : IObservable<T>
        where T : IObservableFeature
    {
    }
}
