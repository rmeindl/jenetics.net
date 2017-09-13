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

namespace Jenetics.Internal.Util
{
    public static class Arrays
    {
        public static double[] Revert(double[] array)
        {
            for (int i = 0, j = array.Length - 1; i < j; ++i, --j)
                Swap(array, i, j);

            return array;
        }

        public static void Swap(int[] array, int i, int j)
        {
            var temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        public static void Swap(double[] array, int i, int j)
        {
            var temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        public static int[] Shuffle(int[] array)
        {
            return Shuffle(array, RandomRegistry.GetRandom());
        }

        public static int[] Shuffle(int[] array, Random random)
        {
            for (var j = array.Length - 1; j > 0; --j)
                Swap(array, j, random.NextInt(j + 1));
            return array;
        }
    }
}