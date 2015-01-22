using System;

namespace Blacklite.Framework.Features
{
    public interface IFeatureProvider
    {
        T GetFeature<T>() where T : IFeature;
        IObservable<T> GetChangedStream<T>() where T : IFeature;
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IObservable<T> GetChangedStream<T>() where T : IFeature
        {
            throw new NotImplementedException();
        }

        public T GetFeature<T>() where T : IFeature
        {
            throw new NotImplementedException();
        }
    }
}