using Blacklite.Framework.Features.Aspects;
using Blacklite.Framework.Features.Traits;
using System;
using System.Reactive.Subjects;

namespace Blacklite.Framework.Features.Traits
{
    public abstract class ObservableTrait : Trait, IObservableTrait
    {
        public ObservableTrait(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures)
        {
        }
    }
}
