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
using Jenetics.Util;

namespace Jenetics.Internal.Math
{
    public static class Base
    {
        public static long UlpDistance(double a, double b)
        {
            return UlpPosition(a) - UlpPosition(b);
        }

        public static long UlpPosition(double a)
        {
            var t = BitConverter.DoubleToInt64Bits(a);
            if (t < 0)
                t = long.MinValue - t;
            return t;
        }

        public static int[] Subset(int n, int k)
        {
            return Subset(n, k, RandomRegistry.GetRandom());
        }

        public static int[] Subset(int n, int k, Random random)
        {
            if (k <= 0)
                throw new ArgumentOutOfRangeException($"Subset size smaller or equal zero: {k}");
            if (n < k)
                throw new ArgumentOutOfRangeException($"n smaller than k: {n} < {k}.");
            var sub = new int[k];
            Subset(n, sub, random);
            return sub;
        }

        public static int[] Subset(int n, int[] sub, Random random)
        {
            var k = sub.Length;
            CheckSubSet(n, k);

            if (sub.Length == n)
            {
                for (var i = 0; i < k; ++i)
                    sub[i] = i;
                return sub;
            }

            for (var i = 0; i < k; ++i)
                sub[i] = i * n / k;

            int l;
            int ix;
            for (var i = 0; i < k; ++i)
            {
                do
                {
                    ix = NextInt(random, 1, n);
                    l = (ix * k - 1) / n;
                } while (sub[l] >= ix);

                sub[l] = sub[l] + 1;
            }

            var m = 0;
            var ip = 0;
            var @is = k;
            for (var i = 0; i < k; ++i)
            {
                m = sub[i];
                sub[i] = 0;

                if (m != i * n / k)
                {
                    ip = ip + 1;
                    sub[ip - 1] = m;
                }
            }

            var ihi = ip;
            for (var i = 1; i <= ihi; ++i)
            {
                ip = ihi + 1 - i;
                l = 1 + (sub[ip - 1] * k - 1) / n;
                var ids = sub[ip - 1] - (l - 1) * n / k;
                sub[ip - 1] = 0;
                sub[@is - 1] = l;
                @is = @is - ids;
            }

            var ir = 0;
            var m0 = 0;
            for (var ll = 1; ll <= k; ++ll)
            {
                l = k + 1 - ll;

                if (sub[l - 1] != 0)
                {
                    ir = l;
                    m0 = 1 + (sub[l - 1] - 1) * n / k;
                    m = sub[l - 1] * n / k - m0 + 1;
                }

                ix = NextInt(random, m0, m0 + m - 1);

                var i = l + 1;
                while (i <= ir && ix >= sub[i - 1])
                {
                    ix = ix + 1;
                    sub[i - 2] = sub[i - 1];
                    i = i + 1;
                }

                sub[i - 2] = ix;
                --m;
            }

            return sub;
        }

        public static void CheckSubSet(int n, int k)
        {
            if (k <= 0)
                throw new ArgumentOutOfRangeException($"Subset size smaller or equal zero: {k}");
            if (n < k)
                throw new ArgumentOutOfRangeException($"n smaller than k: {n} < {k}.");
            if (!Arithmetics.IsMultiplicationSave(n, k))
                throw new ArgumentException(
                    $"n*sub.length > Integer.MAX_VALUE ({n}*{k} = {n * k} > Integer.MAX_VALUE)");
        }

        public static double Clamp(double v, double lo, double hi)
        {
            return v < lo ? lo : v > hi ? hi : v;
        }

        private static int NextInt(Random random, int a, int b)
        {
            return a == b ? a - 1 : random.NextInt(b - a) + a;
        }
    }
}