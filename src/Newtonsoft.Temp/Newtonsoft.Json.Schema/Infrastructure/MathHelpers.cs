﻿#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Temp.Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temp.Newtonsoft.Json.Schema.Infrastructure
{
    internal static class MathHelpers
    {
        public static double FloatingPointRemainder(double dividend, double divisor)
        {
            return dividend - Math.Floor(dividend / divisor) * divisor;
        }

        public static bool IsZero(double value)
        {
            const double epsilon = 2.2204460492503131e-016;

            return Math.Abs(value) < 20.0 * epsilon;
        }
    }
}
