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

namespace Jenetics.Internal.Util
{
    public abstract class IndexSorter
    {
        private const int InsertionSortThreshold = 80;

        protected abstract int[] Sort(double[] array, int[] indexes);

        public static int[] Sort(double[] array)
        {
            var sorter = array.Length < InsertionSortThreshold
                ? InsertionSorter.Instance
                : (IndexSorter) HeapSorter.Instance;

            return sorter.Sort(array, Indexes(array.Length));
        }

        private static int[] Indexes(int length)
        {
            var indexes = new int[length];
            for (var i = 0; i < length; ++i)
                indexes[i] = i;
            return indexes;
        }
    }

    internal class InsertionSorter : IndexSorter
    {
        internal static readonly InsertionSorter Instance = new InsertionSorter();

        protected override int[] Sort(double[] array, int[] indexes)
        {
            for (int i = 1, n = array.Length; i < n; ++i)
            {
                var j = i;
                while (j > 0)
                {
                    if (array[indexes[j - 1]] < array[indexes[j]])
                        Arrays.Swap(indexes, j - 1, j);
                    else
                        break;
                    --j;
                }
            }

            return indexes;
        }
    }

    internal class HeapSorter : IndexSorter
    {
        internal static readonly HeapSorter Instance = new HeapSorter();

        protected override int[] Sort(double[] array, int[] indexes)
        {
            for (var k = array.Length / 2; k >= 0; --k)
                Sink(array, indexes, k, array.Length);

            for (var i = array.Length; --i >= 1;)
            {
                Arrays.Swap(indexes, 0, i);
                Sink(array, indexes, 0, i);
            }

            return indexes;
        }

        private static void Sink(
            double[] array,
            int[] indexes,
            int start,
            int end
        )
        {
            var m = start;
            while (2 * m < end)
            {
                var j = 2 * m;
                if (j < end - 1 && array[indexes[j]] > array[indexes[j + 1]]) ++j;
                if (array[indexes[m]] <= array[indexes[j]]) break;
                Arrays.Swap(indexes, m, j);
                m = j;
            }
        }
    }
}