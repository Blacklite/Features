using System;
using System.Collections;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public interface IFeatureDescriberEnumerable : IEnumerable<IFeatureDescriber>
    {

    }

    public class DefaultFeatureDescriberEnumerable : IFeatureDescriberEnumerable
    {
        public DefaultFeatureDescriberEnumerable()
        {

        }

        public IEnumerator<IFeatureDescriber> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}