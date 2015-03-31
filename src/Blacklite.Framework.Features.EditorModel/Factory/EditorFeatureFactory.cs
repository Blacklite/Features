using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using System;
using Blacklite.Framework.Features.Composition;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.Editors.Factory
{
    public class EditorFeatureFactory : FeatureFactory
    {
        public EditorFeatureFactory(IFeatureCompositionProvider featureCompositionProvider,
            IFeatureDescriberProvider featureDescriberProvider)
            : base(featureCompositionProvider, featureDescriberProvider)
        {
        }

        protected override IEnumerable<IFeatureComposition> GetComposers(Type featureType)
        {
            // filter out required composer, because we need to be able to present the "actual state" to the user.
            return base.GetComposers(featureType).Where(x => x.GetType() != typeof(RequiredFeatureComposer));
        }
    }
}
