using System;

namespace Blacklite.Framework.Features
{
    // Create analyzier to identify miss used IObservableFeatures
    public interface IObservableAspect : IAspect { }
    public interface IObservableFeature : IObservableAspect, IFeature { }

    public interface IObservableAspect<T> : IObservable<T>
        where T : IObservableAspect
    {
    }
}
