// Java Genetic Algorithm Library.
// Copyright (c) 2017 Franz Wilhelmstötter
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Author:
//    Franz Wilhelmstötter (franz.wilhelmstoetter@gmx.at)

using System;
using System.Collections.Generic;
using System.Threading;
using static Jenetics.Internal.Util.Require;

namespace Jenetics.Util
{
    public static class RandomRegistry
    {
        private static readonly Context<Func<Random>> Context = new Context<Func<Random>>(() => new Random());

        public static Random GetRandom()
        {
            return Context.Get()();
        }

        public static void SetRandom(ThreadLocal<Random> random)
        {
            NonNull(random, "Random must not be null.");
            Context.Set(() => random.Value);
        }

        public static void Using<TRandom>(TRandom random, Action<TRandom> consumer)
            where TRandom : Random
        {
            Context.With<TRandom>(() => random, r =>
            {
                consumer(random);
                return null;
            });
        }

        public static bool NextBoolean(this Random random)
        {
            return random.Next(0, 2) != 0;
        }

        public static byte NextByte(this Random random)
        {
            return (byte) random.Next(0, 8);
        }

        public static int NextInt(this Random random)
        {
            return random.Next(int.MinValue, int.MaxValue);
        }

        public static int NextInt(this Random random, int maxValue)
        {
            return random.Next(maxValue);
        }

        public static int NextInt(this Random random, int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public static long NextLong(this Random random, long minValue, long maxValue)
        {
            if (maxValue <= minValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), "maxValue must be > minValue!");

            var uRange = (ulong) (maxValue - minValue);

            ulong ulongRand;
            do
            {
                var buf = new byte[8];
                random.NextBytes(buf);
                ulongRand = (ulong) BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - (ulong.MaxValue % uRange + 1) % uRange);

            return (long) (ulongRand % uRange) + minValue;
        }

        public static long NextLong(this Random random, long maxValue)
        {
            return random.NextLong(0, maxValue);
        }

        public static long NextLong(this Random random)
        {
            return random.NextLong(long.MinValue, long.MaxValue);
        }

        public static IEnumerable<double> Doubles(this Random random)
        {
            while (true)
                yield return random.NextDouble();
        }

        public static T With<TRandom, T>(
            TRandom random,
            Func<TRandom, T> function
        )
            where TRandom : Random
        {
            return Context.With(() => random, s => function(random));
        }
    }
}