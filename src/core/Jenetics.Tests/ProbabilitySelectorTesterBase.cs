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
using Jenetics.Util;
using Xunit;
using static Jenetics.Internal.Util.Require;

namespace Jenetics
{
    public abstract class ProbabilitySelectorTesterBase<TSelector> : SelectorTesterBase<TSelector>
        where TSelector : ProbabilitySelectorBase<DoubleGene, double>
    {
        protected abstract bool IsSorted();

        [Theory]
        [MemberData(nameof(ProbabilitySizes))]
        public void IndexOf2(int size)
        {
            var random = RandomRegistry.GetRandom();

            var props = new double[size];
            var divisor = props.Length * (props.Length + 1) / 2.0;
            for (var i = 0; i < props.Length; ++i)
                props[i] = (i + 1) / divisor;
            Randomize(props, random);

            var incremental = ProbabilitySelector.Incremental((double[]) props.Clone());

            const int samples = 100000;
            for (var i = 0; i < samples; ++i)
            {
                var value = random.NextDouble();
                var index1 = ProbabilitySelector.IndexOf(incremental, value);
                var index2 = IndexOf(props, value);

                Assert.Equal(index2, index1);
            }
        }

        public static IEnumerable<object[]> ProbabilitySizes()
        {
            return new List<object[]>
            {
                new object[] {1},
                new object[] {2},
                new object[] {3},
                new object[] {5},
                new object[] {9},
                new object[] {15},
                new object[] {30},
                new object[] {99},
                new object[] {150}
            };
        }

        private static int IndexOf(double[] array, double value)
        {
            var j = 0;
            double sum = 0;
            for (var i = 0; sum < value && i < array.Length; ++i)
            {
                sum += array[i];
                j = i;
            }
            return j;
        }

        private static void Randomize(double[] array, Random random)
        {
            NonNull(array, "Array");
            for (var j = array.Length - 1; j > 0; --j)
                Swap(array, j, random.NextInt(j + 1));
        }

        private static void Swap(double[] array, int i, int j)
        {
            var temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        protected static void AssertPositive(double[] array)
        {
            foreach (var t in array)
                Assert.True(t >= 0.0, "All values must be positive: " + t);
        }

        [Fact]
        public void IndexOf1()
        {
            var random = RandomRegistry.GetRandom();

            var props = new double[10];
            var divisor = props.Length * (props.Length + 1) / 2.0;
            for (var i = 0; i < props.Length; ++i)
                props[i] = (i + 1) / divisor;
            Randomize(props, random);

            var incremental = ProbabilitySelector.Incremental((double[]) props.Clone());

            const double samples = 1000000;
            var indices = new double[props.Length];

            Array.Fill(indices, 0);

            for (var i = 0; i < samples; ++i)
                indices[ProbabilitySelector.IndexOf(incremental, random.NextDouble())] += 1;

            for (var i = 0; i < props.Length; ++i)
                indices[i] /= samples;

            for (var i = 0; i < props.Length; ++i)
                Assert.Equal(props[i], indices[i], 1);
        }

        [Fact]
        public void Probabilities()
        {
            var population = TestUtils.NewDoublePopulation(100);
            Lists.Shuffle(population, new Random());

            var selector = Factory()();
            var props = selector.Probabilities(population, 23);
            Assert.Equal(population.Count, props.Length);

            Assert.Equal(1.0, props.Sum(), 1);
            AssertPositive(props);
        }
    }
}