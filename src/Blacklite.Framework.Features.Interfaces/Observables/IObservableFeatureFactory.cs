using System;

namespace Blacklite.Framework.Features.Observables
{

    public interface IObservableFeatureFactory
    {
        ObservableFeature<T> GetObservableFeature<T>()
            where T : class, IObservableFeature, new();
    }
}
