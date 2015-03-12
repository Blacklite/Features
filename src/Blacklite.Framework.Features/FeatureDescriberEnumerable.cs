using System;
using System.Collections;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public class DefaultFeatureDescriberEnumerable : IFeatureDescriberEnumerable
    {
        private readonly IFeatureDescriberProvider _describerProvider;
        public DefaultFeatureDescriberEnumerable(IFeatureDescriberProvider describerProvider)
        {
            _describerProvider = describerProvider;
        }

        public IEnumerator<IFeatureDescriber> GetEnumerator()
        {
            return _describerProvider.Describers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _describerProvider.Describers.Values.GetEnumerator();
        }
    }
}
