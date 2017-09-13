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
using System.Linq;
using Jenetics.Internal.Util;
using Jenetics.Util;
using static System.Double;
using static Jenetics.Internal.Util.Require;

namespace Jenetics.Internal.Math
{
    public static class random
    {
        public static int Seed()
        {
            return Seed(DateTime.Now.Ticks);
        }

        public static int Seed(long @base)
        {
            return (int) Mix(@base, ObjectHashSeed());
        }

        public static IEnumerable<int> Indexes(Random randomSource, int n, double p)
        {
            return Indexes(randomSource, 0, n, p);
        }

        public static IEnumerable<int> Indexes(Random randomSource, int start, int end, double p)
        {
            Probability(p);
            var pAsInt = Probability.ToInt(p);

            return Equals(p, 0, 1E-20)
                ? Enumerable.Empty<int>()
                : Equals(p, 1, 1E-20)
                    ? Enumerable.Range(start, end)
                    : Enumerable.Range(start, end).Where(i => randomSource.NextInt() < pAsInt);
        }

        private static bool Equals(double a, double b, double delta)
        {
            return System.Math.Abs(a - b) <= delta;
        }

        private static long Mix(long a, long b)
        {
            var c = a ^ b;
            c ^= c << 17;
            c ^= Bits.bit_rol(c, 31);
            c ^= c << 8;
            return c;
        }

        private static long ObjectHashSeed()
        {
            return (long) ((uint) new object().GetHashCode() << 32) | (uint) new object().GetHashCode();
        }

        public static double NextDouble(Random random, double min, double max)
        {
            if (min >= max)
                throw new ArgumentOutOfRangeException($"min >= max: {min} >= {max}.");

            var value = random.NextDouble();
            if (min < max)
            {
                value = value * (max - min) + min;
                if (value >= max)
                    value = NextDown(value);
            }

            return value;
        }

        private static double NextDown(double d)
        {
            if (IsNaN(d) || IsNegativeInfinity(d))
                return d;
            if (d.CompareTo(0.0) == 0)
                return -MinValue;
            return BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(d) + (d > 0.0d ? -1L : +1L));
        }

        public static int NextInt(Random random, int min, int max)
        {
            if (min > max)
                throw new ArgumentException($"Min >= max: {min} >= {max}");

            var diff = max - min + 1;
            int result;

            if (diff <= 0)
                do
                {
                    result = random.NextInt();
                } while (result < min || result > max);
            else
                result = random.NextInt(diff) + min;

            return result;
        }

        public static char NextCharacter(Random random)
        {
            char c;
            do
            {
                c = (char) NextInt(random, char.MinValue, char.MaxValue);
            } while (!char.IsLetterOrDigit(c));

            return c;
        }

        public static string NextString(Random random, int length)
        {
            var chars = new char[length];
            for (var i = 0; i < length; ++i)
                chars[i] = NextCharacter(random);

            return new string(chars);
        }

        public static long NextLong(
            Random random,
            long min, long max
        )
        {
            if (min > max)
                throw new ArgumentException($"min >= max: {min} >= {max}.");

            var diff = max - min + 1;
            long result;

            if (diff <= 0)
                do
                {
                    result = random.NextLong();
                } while (result < min || result > max);
            else if (diff < int.MaxValue)
                result = random.NextInt((int) diff) + min;
            else
                result = NextLong(random, diff) + min;

            return result;
        }

        public static long NextLong(Random random, long n)
        {
            if (n <= 0)
                throw new ArgumentException($"n is smaller than one: {n}");

            long bits;
            long result;
            do
            {
                bits = random.NextLong() & 0x7fffffffffffffffL;
                result = bits % n;
            } while (bits - result + (n - 1) < 0);

            return result;
        }
    }
}