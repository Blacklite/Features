﻿using Blacklite.Framework.Features.OptionModel;
using Blacklite.Framework.Features.Traits;
using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public sealed partial class Feature
    {
        /// <summary>
        /// Allows a feature to behave randomly
        /// </summary>
        public class Random : ITrait
        {
            public Random()
            {
                IsEnabled = RandomGenerator.Next() % 2 == 0;
            }

            public bool IsEnabled { get; }

            // Based on: http://blogs.msdn.com/b/pfxteam/archive/2009/02/19/9434171.aspx
            private static class RandomGenerator
            {
                private static readonly System.Random NonThreadLocalInstance = new System.Random();

                [ThreadStatic]
                private static System.Random _threadLocalInstance;

                public static int Next()
                {
                    var rnd = _threadLocalInstance;

                    if (rnd != null)
                    {
                        return rnd.Next();
                    }

                    int seed;

                    lock (NonThreadLocalInstance) seed = NonThreadLocalInstance.Next();

                    _threadLocalInstance = rnd = new System.Random(seed);

                    return rnd.Next();
                }
            }
        }

        public class Random<TOptions> : Random, ITrait<TOptions>
            where TOptions : class, new()
        {
            public TOptions Options { get; private set; }

            void ITraitOptions.SetOptions(object options)
            {
                Options = (TOptions)options;
            }

            void ITrait<TOptions>.SetOptions(TOptions options)
            {
                Options = options;
            }
        }
    }
}
