using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features
{
    public abstract partial class Feature
    {
        /// <summary>
        /// Allows a feature to behave randomly
        /// </summary>
        public class Random : ScopedFeature
        {
            public Random()
            {
                _enabled = RandomGenerator.Next() % 2 == 0;
            }

            private bool _enabled;
            public override bool IsEnabled { get { return _enabled; } }

            // Based on: http://blogs.msdn.com/b/pfxteam/archive/2009/02/19/9434171.aspx
            static class RandomGenerator
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
    }
}
