﻿using System;
using System.Collections.Generic;
using Blacklite.Framework.Features.OptionsModel;
using System.ComponentModel.DataAnnotations;

namespace Blacklite.Framework.Features
{
    public interface Feature<out TFeature>
        where TFeature : class
    {
        TFeature Value { get; }
    }
}
