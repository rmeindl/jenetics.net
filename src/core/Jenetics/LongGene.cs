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
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics;

/// <summary>
///     Long integer gene for genetic algorithms.
///     Modernized with primary constructor syntax (C# 12).
/// </summary>
[Serializable]
public class LongGene(long value, long min, long max)
    : NumericGeneBase<long, LongGene>(value, min, max), IMean<LongGene>
{
    public LongGene Mean(LongGene that)
    {
        return new LongGene(Value + (that.Value - Value) / 2, Min, Max);
    }

    public override int CompareTo(LongGene? other)
    {
        return other == null ? 1 : Value.CompareTo(other.Value);
    }

    public override LongGene NewInstance()
    {
        return new LongGene(
            RandomExtensions.NextLong(RandomRegistry.GetRandom(), Min, Max), Min, Max);
    }

    public override LongGene NewInstance(long value)
    {
        return new LongGene(value, Min, Max);
    }

    internal static IImmutableSeq<LongGene> Seq(
        long minimum,
        long maximum,
        int length
    )
    {
        Require.Positive(length);

        var min = minimum;
        var max = maximum;
        var r = RandomRegistry.GetRandom();

        return MutableSeq.OfLength<LongGene>(length)
            .Fill(() => new LongGene(RandomExtensions.NextLong(r, min, max), minimum, maximum))
            .ToImmutableSeq();
    }

    public static LongGene Of(long min, long max)
    {
        return Of(RandomExtensions.NextLong(RandomRegistry.GetRandom(), min, max), min, max);
    }

    public static LongGene Of(long value, long min, long max)
    {
        return new LongGene(value, min, max);
    }
}