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

namespace Jenetics.Util;

public abstract class Random64 : Random
{
    protected Random64() : base(0)
    {
    }

    protected Random64(long seed) : base(0)
    {
    }

    public abstract long NextLong();

    public virtual void SetSeed(long seed)
    {
        throw new NotSupportedException("This random generator does not support reseeding.");
    }

    public override long NextInt64()
    {
        return NextLong();
    }

    public override long NextInt64(long maxValue)
    {
        return NextLong(maxValue);
    }

    public override long NextInt64(long minValue, long maxValue)
    {
        return NextLong(minValue, maxValue);
    }

    public long NextLong(long maxValue)
    {
        if (maxValue <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxValue), "maxValue must be positive.");

        return (long)NextBounded((ulong)maxValue);
    }

    public long NextLong(long minValue, long maxValue)
    {
        if (maxValue <= minValue)
            throw new ArgumentOutOfRangeException(nameof(maxValue), "maxValue must be greater than minValue.");

        var range = unchecked((ulong)maxValue - (ulong)minValue);
        var offset = NextBounded(range);
        return unchecked((long)((ulong)minValue + offset));
    }

    public override int Next()
    {
        return (int)((ulong)NextLong() >> 33);
    }

    public override int Next(int maxValue)
    {
        if (maxValue < 0)
            throw new ArgumentOutOfRangeException(nameof(maxValue));

        return maxValue == 0 ? 0 : (int)NextBounded((ulong)maxValue);
    }

    public override int Next(int minValue, int maxValue)
    {
        if (maxValue < minValue)
            throw new ArgumentOutOfRangeException(nameof(maxValue));
        if (maxValue == minValue)
            return minValue;

        var range = (uint)((long)maxValue - minValue);
        return (int)((long)minValue + (long)NextBounded(range));
    }

    public override double NextDouble()
    {
        return ((ulong)NextLong() >> 11) * (1.0 / (1UL << 53));
    }

    public override float NextSingle()
    {
        return (float)((ulong)NextLong() >> 40) * (1.0f / (1U << 24));
    }

    public override void NextBytes(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        NextBytes(buffer.AsSpan());
    }

    public override void NextBytes(Span<byte> buffer)
    {
        var index = 0;
        while (index < buffer.Length)
        {
            var value = (ulong)NextLong();
            var count = Math.Min(sizeof(long), buffer.Length - index);
            for (var i = 0; i < count; ++i)
            {
                buffer[index++] = (byte)value;
                value >>= 8;
            }
        }
    }

    protected override double Sample()
    {
        return NextDouble();
    }

    public static Random64 Of(Func<long> supplier)
    {
        ArgumentNullException.ThrowIfNull(supplier);
        return new SupplierRandom(supplier);
    }

    private ulong NextBounded(ulong bound)
    {
        var threshold = unchecked(0UL - bound) % bound;
        while (true)
        {
            var value = (ulong)NextLong();
            if (value >= threshold)
                return value % bound;
        }
    }

    private sealed class SupplierRandom(Func<long> supplier) : Random64
    {
        public override long NextLong()
        {
            return supplier();
        }
    }
}
