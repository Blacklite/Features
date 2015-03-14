using System;
using System.Collections.Generic;
using Microsoft.Framework.DependencyInjection;
using System.Linq;
using Blacklite.Framework.Features.OptionsModel;
using System.ComponentModel.DataAnnotations;

namespace Blacklite.Framework.Features
{
    public interface Feature<out TAspect>
        where TAspect : class
    {
        TAspect Value { get; }
    }
}
