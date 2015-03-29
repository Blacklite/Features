using System;

namespace Blacklite.Framework.Features
{
    public interface ObservableFeature<out T> : IObservable<T>
        where T : IObservableFeature
    {
    }
}
