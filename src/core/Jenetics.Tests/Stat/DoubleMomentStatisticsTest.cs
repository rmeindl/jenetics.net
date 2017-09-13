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
using MathNet.Numerics.Statistics;
using Xunit;

namespace Jenetics.Stat
{
    public class DoubleMomentStatisticsTest
    {
        private static IList<double> Numbers(int size)
        {
            var random = new Random(123);
            var numbers = new List<double>(size);
            for (var i = 0; i < size; ++i)
                numbers.Add(random.NextDouble());

            return numbers;
        }

        [Theory]
        [MemberData(nameof(SampleCounts))]
        public void Summary(int sampleCounts, double epsilon)
        {
            var numbers = Numbers(sampleCounts);

            var expected = new DescriptiveStatistics(numbers);

            var summary = numbers.ToDoubleMomentStatistics(n => n);

            Assert.Equal(summary.Count, numbers.Count);
            AssertEqualsDouble(Min(summary.Min), expected.Minimum, 0.0);
            AssertEqualsDouble(Max(summary.Max), expected.Maximum, 0.0);
            AssertEqualsDouble(summary.Sum, numbers.Sum(), epsilon);
            AssertEqualsDouble(summary.Mean, expected.Count == 0 ? double.NaN : expected.Mean, epsilon);
            AssertEqualsDouble(summary.Variance, expected.Count == 1 ? 0.0 : expected.Variance, epsilon);
            AssertEqualsDouble(summary.Skewness, expected.Skewness, epsilon);
            AssertEqualsDouble(summary.Kurtosis, expected.Kurtosis, epsilon);
        }

        private static double Min(double value)
        {
            return double.IsPositiveInfinity(value) ? double.NaN : value;
        }

        private static double Max(double value)
        {
            return double.IsNegativeInfinity(value) ? double.NaN : value;
        }

        private static void AssertEqualsDouble(double a, double expected, double e)
        {
            if (double.IsNaN(expected))
                Assert.True(
                    double.IsNaN(a),
                    $"Expected: Double.NaN \nActual: {a}"
                );
            else
                Assert.Equal(expected, a, 5);
        }

        public static IEnumerable<object[]> SampleCounts()
        {
            yield return new object[] {0, 0.0};
            yield return new object[] {1, 0.0};
            yield return new object[] {2, 0.05};
            yield return new object[] {3, 0.05};
            yield return new object[] {100, 0.05};
            yield return new object[] {1_000, 0.0001};
            yield return new object[] {10_000, 0.00001};
            yield return new object[] {100_000, 0.000001};
            yield return new object[] {1_000_000, 0.0000001};
            yield return new object[] {2_000_000, 0.0000005};
        }

        [Fact]
        public void SameState()
        {
            var dms1 = new DoubleMomentStatistics();
            var dms2 = new DoubleMomentStatistics();

            var random = new Random();
            for (var i = 0; i < 100; ++i)
            {
                var value = random.NextDouble();
                dms1.Accept(value);
                dms2.Accept(value);

                Assert.True(dms1.SameState(dms2));
                Assert.True(dms2.SameState(dms1));
                Assert.True(dms1.SameState(dms1));
                Assert.True(dms2.SameState(dms2));
            }
        }
    }
}