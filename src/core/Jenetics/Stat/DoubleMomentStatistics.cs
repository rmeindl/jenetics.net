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
using Jenetics.Internal.Util;

namespace Jenetics.Stat
{
    public class DoubleMomentStatistics : MomentStatistics
    {
        private readonly DoubleAdder _sum = new DoubleAdder();

        public double Sum => _sum.DoubleValue;

        public double Min { get; private set; } = double.PositiveInfinity;

        public double Max { get; private set; } = double.NegativeInfinity;

        public new void Accept(double value)
        {
            base.Accept(value);
            Min = Math.Min(Min, value);
            Max = Math.Max(Max, value);
            _sum.Add(value);
        }

        public override string ToString()
        {
            return
                $"Summary[N={Count}, ∧={Min}, ∨={Max}, Σ={_sum}, μ={Mean}, s²={Variance}, S={Skewness}, K={Kurtosis}]";
        }

        public bool SameState(DoubleMomentStatistics other)
        {
            return Min.CompareTo(other.Min) == 0 &&
                   Max.CompareTo(other.Max) == 0 &&
                   _sum.SameState(other._sum) &&
                   base.SameState(other);
        }
    }

    public static class DoubleMomentStatisticsExtensions
    {
        public static DoubleMomentStatistics ToDoubleMomentStatistics<T>(
            this IEnumerable<T> values, Func<T, double> mapper)
        {
            Require.NonNull(mapper);
            var statistics = new DoubleMomentStatistics();
            foreach (var v in values)
                statistics.Accept(mapper(v));
            return statistics;
        }
    }
}