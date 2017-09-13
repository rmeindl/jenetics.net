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
using Jenetics.Internal.Math;
using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public class ProbabilitySelectorTest
    {
        private static double[] Array(int size, Random randomSource)
        {
            var array = new double[size];
            for (var i = 0; i < array.Length; ++i)
                array[i] = i;

            Lists.Shuffle(array, randomSource);
            return array;
        }

        [Theory]
        [MemberData(nameof(ArraySize))]
        public void Revert(int size)
        {
            var probabilities = Array(size, new Random());
            var reverted = ProbabilitySelector.SortAndRevert(probabilities);

            for (var i = 0; i < size; ++i)
                Assert.Equal(size - 1.0, probabilities[i] + reverted[i]);
        }

        [Theory]
        [MemberData(nameof(ArraySize))]
        public void RevertSortedArray(int size)
        {
            var values = Array(size, new Random());
            System.Array.Sort(values);

            var reverted = ProbabilitySelector.SortAndRevert(values);
            for (var i = 0; i < values.Length; ++i)
                Assert.Equal(values.Length - i - 1, reverted[i]);
        }

        [Theory]
        [MemberData(nameof(ArraySize))]
        public void IndexOfSerialEqualBinary(int size)
        {
            var probabilities = Array(size, new Random(12));
            Arithmetics.Normalize(probabilities);
            ProbabilitySelector.Incremental(probabilities);

            Assert.Equal(
                ProbabilitySelector.IndexOfSerial(probabilities, 0.5),
                ProbabilitySelector.IndexOfBinary(probabilities, 0.5)
            );
        }

        public static IEnumerable<object[]> ArraySize()
        {
            return new[]
            {
                new object[] {6},
                new object[] {100},
                new object[] {1000},
                new object[] {10_000},
                new object[] {100_000},
                new object[] {500_000}
            };
        }
    }
}