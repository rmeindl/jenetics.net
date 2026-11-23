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
using static Jenetics.Internal.Util.Require;

namespace Jenetics.Stat;

public sealed class MinMax<T>
{
    private readonly IComparer<T> _comparer;

    private T? _min;
    private T? _max;

    private MinMax(IComparer<T> comparer)
    {
        _comparer = NonNull(comparer);
    }

    public long Count { get; private set; }

    public T? Min => _min;

    public T? Max => _max;

    public void Accept(T value)
    {
        _min = MinValue(_comparer, _min, value);
        _max = MaxValue(_comparer, _max, value);
        ++Count;
    }

    public MinMax<T> Combine(MinMax<T> other)
    {
        NonNull(other);
        _min = MinValue(_comparer, _min, other._min);
        _max = MaxValue(_comparer, _max, other._max);
        Count += other.Count;
        return this;
    }

    public bool SameState(MinMax<T> other)
    {
        NonNull(other);
        return Count == other.Count &&
               EqualityComparer<T?>.Default.Equals(_min, other._min) &&
               EqualityComparer<T?>.Default.Equals(_max, other._max);
    }

    public static MinMax<T> Of(IComparer<T>? comparer = null)
    {
        return new MinMax<T>(comparer ?? Comparer<T>.Default);
    }

    public static T? MinValue(IComparer<T> comparer, T? a, T? b)
    {
        NonNull(comparer);
        return a is not null
            ? b is not null && comparer.Compare(a, b) > 0 ? b : a
            : b;
    }

    public static T? MaxValue(IComparer<T> comparer, T? a, T? b)
    {
        NonNull(comparer);
        return a is not null
            ? b is not null && comparer.Compare(a, b) < 0 ? b : a
            : b;
    }
}

public static class MinMaxExtensions
{
    public static MinMax<T> ToMinMax<T>(this IEnumerable<T> values, IComparer<T>? comparer = null)
    {
        NonNull(values);
        var result = MinMax<T>.Of(comparer);
        foreach (var value in values)
            result.Accept(value);
        return result;
    }
}
