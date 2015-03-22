using System;

namespace Blacklite.Framework.Features.Observables
{
    public interface IFeatureSubjectFactory
    {
        IFeatureSubject GetSubject(Type featureType);
    }

    public interface ISingletonFeatureSubjectFactory : IFeatureSubjectFactory { }
}
