using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.Describers
{
    public class FeatureDescriberCollection : IEnumerable<IFeatureDescriber>
    {
        private readonly IEnumerable<IFeatureDescriber> _describers;
        public FeatureDescriberCollection(IEnumerable<IFeatureDescriber> describers)
        {
            _describers = describers.ToArray();
        }

        public IEnumerator<IFeatureDescriber> GetEnumerator()
        {
            return _describers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _describers.GetEnumerator();
        }
    }
}
