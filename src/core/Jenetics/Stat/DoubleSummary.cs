// Java Genetic Algorithm Library.
// Copyright (c) 2017 Franz Wilhelmstötter
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using static Jenetics.Internal.Util.Require;

namespace Jenetics.Stat;

public sealed record DoubleSummary(
    long Count,
    double Min,
    double Max,
    double Sum,
    double Mean
)
{
    public static DoubleSummary Of(DoubleMomentStatistics statistics)
    {
        NonNull(statistics);
        return new DoubleSummary(
            statistics.Count,
            statistics.Min,
            statistics.Max,
            statistics.Sum,
            statistics.Count == 0 ? 0.0 : statistics.Mean
        );
    }

    public static DoubleSummary Of(IEnumerable<double> values)
    {
        NonNull(values);
        var statistics = new DoubleMomentStatistics();
        foreach (var value in values)
            statistics.Accept(value);
        return Of(statistics);
    }

    public static DoubleSummary ToDoubleSummary<T>(
        IEnumerable<T> values,
        Func<T, double> mapper
    )
    {
        NonNull(values);
        NonNull(mapper);
        var statistics = new DoubleMomentStatistics();
        foreach (var value in values)
            statistics.Accept(mapper(value));
        return Of(statistics);
    }
}
