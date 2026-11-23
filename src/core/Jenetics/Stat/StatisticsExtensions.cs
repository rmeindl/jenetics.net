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
using System.Collections.Generic;

namespace Jenetics.Stat;

public static class StatisticsExtensions
{
    public static Quantile ToQuantile<T>(
        this IEnumerable<T> values,
        double quantile,
        Func<T, double> mapper
    )
    {
        return Quantile.ToQuantile(values, quantile, mapper);
    }

    public static Quantile ToMedian<T>(this IEnumerable<T> values, Func<T, double> mapper)
    {
        return Quantile.ToMedian(values, mapper);
    }

    public static DoubleSummary ToDoubleSummary<T>(
        this IEnumerable<T> values,
        Func<T, double> mapper
    )
    {
        return DoubleSummary.ToDoubleSummary(values, mapper);
    }

    public static IntSummary ToIntSummary<T>(
        this IEnumerable<T> values,
        Func<T, int> mapper
    )
    {
        return IntSummary.ToIntSummary(values, mapper);
    }

    public static LongSummary ToLongSummary<T>(
        this IEnumerable<T> values,
        Func<T, long> mapper
    )
    {
        return LongSummary.ToLongSummary(values, mapper);
    }
}
