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
using Jenetics.Internal.Math;

namespace Jenetics.Stat;

public abstract class MomentStatistics
{
    private readonly DoubleAdder _m1 = new();
    private readonly DoubleAdder _m2 = new();
    private readonly DoubleAdder _m3 = new();
    private readonly DoubleAdder _m4 = new();

    public double Mean => Count == 0L ? double.NaN : _m1.Value;

    public long Count { get; private set; }

    public double Variance
    {
        get
        {
            var var = double.NaN;
            if (Count == 1L)
                var = _m2.Value;
            else if (Count > 1L)
                var = _m2.Value / (Count - 1.0);

            return var;
        }
    }

    public double Skewness
    {
        get
        {
            var skewness = double.NaN;
            if (Count >= 3L)
            {
                var var = _m2.Value / (Count - 1.0);
                if (var < 10E-20)
                    skewness = 0.0d;
                else
                    skewness = Count * _m3.Value /
                               ((Count - 1.0) * (Count - 2.0) * Math.Sqrt(var) * var);
            }

            return skewness;
        }
    }

    public double Kurtosis
    {
        get
        {
            var kurtosis = double.NaN;
            if (Count > 3L)
            {
                var var = _m2.Value / (Count - 1);
                if (Count <= 3L || var < 10E-20)
                    kurtosis = 0.0;
                else
                    kurtosis = (Count * (Count + 1.0) * _m4.Value -
                                3.0 * _m2.Value * _m2.Value * (Count - 1.0)) /
                               ((Count - 1.0) * (Count - 2.0) * (Count - 3.0) * var * var);
            }

            return kurtosis;
        }
    }

    protected void Accept(double value)
    {
        ++Count;

        double n = Count;
        var d = value - _m1.Value;
        var dN = d / n;
        var dN2 = dN * dN;
        var t1 = d * dN * (n - 1.0);

        _m1.Add(dN);
        _m4.Add(t1 * dN2 * (n * n - 3.0 * n + 3.0))
            .Add(6.0 * dN2 * _m2.Value - 4.0 * dN * _m3.Value);
        _m3.Add(t1 * dN * (n - 2.0) - 3.0 * dN * _m2.Value);
        _m2.Add(t1);
    }

    protected void Combine(MomentStatistics other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (other.Count == 0)
            return;

        if (Count == 0)
        {
            Count = other.Count;
            _m1.Add(other._m1.Value);
            _m2.Add(other._m2.Value);
            _m3.Add(other._m3.Value);
            _m4.Add(other._m4.Value);
            return;
        }

        var otherCount = other.Count;
        var otherMean = other._m1.Value;
        var otherM2 = other._m2.Value;
        var otherM3 = other._m3.Value;
        var otherM4 = other._m4.Value;
        var m2 = _m2.Value;
        var m3 = _m3.Value;
        var previousCount = (double)Count;
        var otherCountValue = (double)otherCount;
        var count = previousCount + otherCountValue;
        var countSquared = count * count;
        var difference = otherMean - _m1.Value;
        var differenceSquared = difference * difference;

        Count += otherCount;
        _m1.Add(difference * otherCountValue / count);
        _m2.Add(otherM2)
            .Add(differenceSquared * previousCount * otherCountValue / count);
        _m3.Add(otherM3)
            .Add(differenceSquared * difference *
                 (previousCount * otherCountValue * (previousCount - otherCountValue) / countSquared))
            .Add(3.0 * difference *
                 (previousCount * otherM2 - otherCountValue * m2) / count);
        _m4.Add(otherM4)
            .Add(differenceSquared * differenceSquared *
                 (previousCount * otherCountValue *
                  (previousCount * previousCount - previousCount * otherCountValue +
                   otherCountValue * otherCountValue) / (countSquared * count)))
            .Add(6.0 * differenceSquared *
                 (previousCount * previousCount * otherM2 +
                  otherCountValue * otherCountValue * m2) / countSquared)
            .Add(4.0 * difference *
                 (previousCount * otherM3 - otherCountValue * m3) / count);
    }

    protected bool SameState(MomentStatistics statistics)
    {
        return Count == statistics.Count &&
               _m1.SameState(statistics._m1) &&
               _m2.SameState(statistics._m2) &&
               _m3.SameState(statistics._m3) &&
               _m4.SameState(statistics._m4);
    }
}