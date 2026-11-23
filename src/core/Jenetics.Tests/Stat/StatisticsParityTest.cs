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

using System;
using Xunit;

namespace Jenetics.Stat;

public class StatisticsParityTest
{
    [Fact]
    public void CombineDoubleMomentStatistics()
    {
        var left = new DoubleMomentStatistics();
        left.Accept(1.0);
        left.Accept(2.0);
        left.Accept(3.0);

        var right = new DoubleMomentStatistics();
        right.Accept(4.0);
        right.Accept(5.0);
        right.Accept(6.0);

        left.Combine(right);

        Assert.Equal(6, left.Count);
        Assert.Equal(1.0, left.Min);
        Assert.Equal(6.0, left.Max);
        Assert.Equal(21.0, left.Sum);
        Assert.Equal(3.5, left.Mean, 12);
        Assert.Equal(3.5, left.Variance, 12);

        var snapshot = left.ToDoubleMoments();
        Assert.Equal(6, snapshot.Count);
        Assert.Equal(left.Min, snapshot.Min);
        Assert.Equal(left.Max, snapshot.Max);
        Assert.Equal(left.Sum, snapshot.Sum);
    }

    [Fact]
    public void CombineDoubleMomentStatisticsWithItself()
    {
        var statistics = new DoubleMomentStatistics();
        statistics.Accept(1.0);
        statistics.Accept(2.0);
        statistics.Accept(4.0);

        statistics.Combine(statistics);

        Assert.Equal(6, statistics.Count);
        Assert.Equal(7.0 / 3.0, statistics.Mean, 12);
        Assert.Equal(14.0, statistics.Sum);
    }

    [Fact]
    public void MinMaxCombinesUsingConfiguredComparer()
    {
        var values = MinMax<string>.Of(StringComparer.OrdinalIgnoreCase);
        values.Accept("b");
        values.Accept("A");

        var other = MinMax<string>.Of(StringComparer.OrdinalIgnoreCase);
        other.Accept("c");
        other.Accept("B");
        values.Combine(other);

        Assert.Equal(4, values.Count);
        Assert.Equal("A", values.Min);
        Assert.Equal("c", values.Max);
    }

    [Fact]
    public void IntegerAndLongStatisticsCombineAndSnapshot()
    {
        var integers = new IntMomentStatistics();
        integers.Accept(1);
        integers.Accept(3);
        var moreIntegers = new IntMomentStatistics();
        moreIntegers.Accept(5);
        integers.Combine(moreIntegers);

        Assert.Equal(3, integers.Count);
        Assert.Equal(9, integers.Sum);
        Assert.Equal(1, integers.Min);
        Assert.Equal(5, integers.Max);
        Assert.Equal(3.0, integers.ToIntMoments().Mean, 12);

        var longs = new LongMomentStatistics();
        longs.Accept(-4);
        longs.Accept(10);
        var moreLongs = new LongMomentStatistics();
        moreLongs.Accept(2);
        longs.Combine(moreLongs);

        Assert.Equal(3, longs.Count);
        Assert.Equal(8, longs.Sum);
        Assert.Equal(-4, longs.Min);
        Assert.Equal(10, longs.Max);
        Assert.Equal(8, longs.ToLongMoments().Sum);
    }

    [Fact]
    public void SummaryAndQuantileExtensionsMapValues()
    {
        var doubleSummary = new[] { 1.0, 2.0, 3.0 }.ToDoubleSummary(value => value * 2);
        Assert.Equal(3, doubleSummary.Count);
        Assert.Equal(12.0, doubleSummary.Sum);
        Assert.Equal(4.0, doubleSummary.Mean);

        var intSummary = new[] { 1, 2, 3 }.ToIntSummary(value => value * 2);
        Assert.Equal(12, intSummary.Sum);
        Assert.Equal(4.0, intSummary.Mean);

        var longSummary = new[] { 1L, 2L, 3L }.ToLongSummary(value => value * 2);
        Assert.Equal(12, longSummary.Sum);
        Assert.Equal(4.0, longSummary.Mean);

        var median = new[] { 1, 2, 3, 4, 5 }.ToMedian(value => value);
        Assert.Equal(5, median.Samples);
        Assert.Equal(3.0, median.Value);
    }

    [Fact]
    public void QuantileTracksEndpointsAndReset()
    {
        var minimum = new Quantile(0.0);
        var maximum = new Quantile(1.0);

        foreach (var value in new[] { 4.0, -2.0, 7.0, 1.0 })
        {
            minimum.Accept(value);
            maximum.Accept(value);
        }

        Assert.Equal(-2.0, minimum.Value);
        Assert.Equal(7.0, maximum.Value);
        Assert.Equal(4, minimum.Samples);

        minimum.Reset();
        Assert.Equal(0, minimum.Samples);
        Assert.True(double.IsNaN(minimum.Value));

        minimum.Accept(3.0);
        Assert.Equal(3.0, minimum.Value);
    }

    [Fact]
    public void QuantileEstimatesMedianForIncreasingSequence()
    {
        var median = new Quantile(0.5);
        for (var value = 0; value < 10_000; ++value)
            median.Accept(value);

        Assert.Equal(10_000, median.Samples);
        Assert.InRange(median.Value, 4_900.0, 5_100.0);
    }

    [Fact]
    public void QuantileCombinesPartialAccumulators()
    {
        var first = new Quantile(0.5);
        first.Accept(1.0);
        first.Accept(3.0);

        var second = new Quantile(0.5);
        second.Accept(2.0);
        second.Accept(4.0);

        first.Combine(second);

        Assert.Equal(4, first.Samples);
        Assert.Equal(2.5, first.Value);
    }
}
