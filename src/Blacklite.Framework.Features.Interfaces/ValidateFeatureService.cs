using System;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features
{
    public interface IValidateFeatureService : IDisposable
    {
        bool Validate();
    }
}
