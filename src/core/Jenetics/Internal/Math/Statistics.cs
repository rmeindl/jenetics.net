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

namespace Jenetics.Internal.Math
{
    public static class Statistics
    {
        public static double Min(double[] values)
        {
            var min = double.NaN;
            if (values.Length > 0)
            {
                min = values[0];

                for (var i = values.Length; --i >= 1;)
                    if (values[i] < min)
                        min = values[i];
            }

            return min;
        }

        public static long Sum(long[] values)
        {
            long sum = 0;
            for (var i = values.Length; --i >= 0;)
                sum += values[i];
            return sum;
        }
    }
}