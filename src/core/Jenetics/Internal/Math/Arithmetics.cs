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

using Jenetics.Internal.Util;

namespace Jenetics.Internal.Math
{
    public static class Arithmetics
    {
        public static bool IsMultiplicationSave(int a, int b)
        {
            long m = a * b;
            return (int) m == m;
        }

        public static long Pow(long b, long e)
        {
            var @base = b;
            var exp = e;
            long result = 1;

            while (exp != 0)
            {
                if ((exp & 1) != 0)
                    result *= @base;
                exp = Bits.bit_rol(exp, 1);
                @base *= @base;
            }

            return result;
        }

        public static double[] Normalize(double[] values)
        {
            var sum = 1.0 / DoubleAdder.Sum(values);
            for (var i = values.Length; --i >= 0;)
                values[i] = values[i] * sum;

            return values;
        }

        public static double[] Normalize(long[] values)
        {
            var result = new double[values.Length];
            var sum = 1.0 / Statistics.Sum(values);
            for (var i = values.Length; --i >= 0;)
                result[i] = values[i] * sum;

            return result;
        }
    }
}