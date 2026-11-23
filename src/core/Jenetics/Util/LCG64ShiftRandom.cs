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
using System.Threading;

namespace Jenetics.Util;

[Obsolete("Use a modern random generator instead.")]
public class LCG64ShiftRandom : Random64, IEquatable<LCG64ShiftRandom>
{
    private long _state;
    private Param _param;

    public LCG64ShiftRandom(Param param, long seed)
    {
        ArgumentNullException.ThrowIfNull(param);
        _param = param;
        _state = seed;
    }

    public LCG64ShiftRandom(Param param) : this(param, Random.Shared.NextInt64())
    {
    }

    public LCG64ShiftRandom(long seed) : this(Param.DEFAULT, seed)
    {
    }

    public LCG64ShiftRandom() : this(Param.DEFAULT)
    {
    }

    public override long NextLong()
    {
        unchecked
        {
            _state = _param.A * _state + _param.B;
            var value = (ulong)_state;
            value ^= value >> 17;
            value ^= value << 31;
            value ^= value >> 8;
            return (long)value;
        }
    }

    public override void SetSeed(long seed)
    {
        _state = seed;
    }

    public virtual void Split(int p, int s)
    {
        if (p < 1)
            throw new ArgumentOutOfRangeException(nameof(p), "p must be at least one.");
        if (s < 0 || s >= p)
            throw new ArgumentOutOfRangeException(nameof(s), "s must be in the range [0, p).");

        if (p > 1)
        {
            Jump(s + 1);
            var param = _param;
            unchecked
            {
                var b = (long)((ulong)param.B * SumPowers((ulong)p, (ulong)param.A));
                var a = Pow((ulong)param.A, (ulong)p);
                _param = Param.Of((long)a, b);
            }
            Backward();
        }
    }

    public virtual void Jump2(int s)
    {
        if ((uint)s >= 64)
            throw new ArgumentOutOfRangeException(nameof(s), "s must be in the range [0, 64).");

        unchecked
        {
            var steps = 1UL << s;
            var a = (ulong)_param.A;
            var b = (ulong)_param.B;
            _state = (long)((ulong)_state * Pow(a, steps) + SumPowers(steps, a) * b);
        }
    }

    public virtual void Jump(long step)
    {
        if (step < 0)
            throw new ArgumentOutOfRangeException(nameof(step), "step must not be negative.");

        if (step < 16)
        {
            for (var i = 0; i < step; ++i)
                Step();
        }
        else
        {
            var steps = (ulong)step;
            var bit = 0;
            while (steps > 0)
            {
                if ((steps & 1) != 0)
                    Jump2(bit);
                ++bit;
                steps >>= 1;
            }
        }
    }

    public Param GetParam()
    {
        return _param;
    }

    public bool Equals(LCG64ShiftRandom? other)
    {
        return other is not null &&
            GetType() == other.GetType() &&
            _param.Equals(other._param) &&
            _state == other._state;
    }

    public override bool Equals(object? obj)
    {
        return obj is LCG64ShiftRandom other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), _param, _state);
    }

    public override string ToString()
    {
        return $"{GetType().Name}[{_param}, State[{_state}]]";
    }

    private void Step()
    {
        unchecked
        {
            _state = _param.A * _state + _param.B;
        }
    }

    private void Backward()
    {
        for (var i = 0; i < 64; ++i)
            Jump2(i);
    }

    private static ulong Pow(ulong value, ulong exponent)
    {
        var result = 1UL;
        while (exponent != 0)
        {
            if ((exponent & 1) != 0)
                result = unchecked(result * value);
            value = unchecked(value * value);
            exponent >>= 1;
        }
        return result;
    }

    private static ulong SumPowers(ulong count, ulong value)
    {
        var sum = 0UL;
        var power = value;
        var bit = 0;
        while (count != 0)
        {
            if ((count & 1) != 0)
                sum = unchecked(ProductSeries(bit, value) + power * sum);
            power = unchecked(power * power);
            ++bit;
            count >>= 1;
        }
        return sum;
    }

    private static ulong ProductSeries(int length, ulong value)
    {
        var product = 1UL;
        var power = value;
        for (var i = 0; i < length; ++i)
        {
            product = unchecked(product * (1 + power));
            power = unchecked(power * power);
        }
        return product;
    }

    [Obsolete("Use a modern random generator instead.")]
    public sealed class ThreadSafe : LCG64ShiftRandom
    {
        private readonly object _sync = new();

        public ThreadSafe(Param param, long seed) : base(param, seed)
        {
        }

        [Obsolete("Use the constructor with Param before seed.")]
        public ThreadSafe(long seed, Param param) : base(param, seed)
        {
        }

        public ThreadSafe(long seed) : base(seed)
        {
        }

        public ThreadSafe(Param param) : base(param)
        {
        }

        public ThreadSafe() : base()
        {
        }

        public override long NextLong()
        {
            lock (_sync)
                return base.NextLong();
        }

        public override void SetSeed(long seed)
        {
            lock (_sync)
                base.SetSeed(seed);
        }

        public override void Split(int p, int s)
        {
            lock (_sync)
                base.Split(p, s);
        }

        public override void Jump2(int s)
        {
            lock (_sync)
                base.Jump2(s);
        }

        public override void Jump(long step)
        {
            lock (_sync)
                base.Jump(step);
        }
    }

    [Obsolete("Use a modern random generator instead.")]
    public sealed class ThreadLocal : System.Threading.ThreadLocal<Random>
    {
        public ThreadLocal() : this(Param.DEFAULT)
        {
        }

        public ThreadLocal(Param param) : base(new ThreadLocalFactory(param).Create)
        {
        }

        public new LCG64ShiftRandom Value => (LCG64ShiftRandom)base.Value!;

        private sealed class ThreadLocalFactory
        {
            private const long StepBase = 1L << 56;
            private readonly object _sync = new();
            private readonly Param _param;
            private int _block;
            private long _seed = Random.Shared.NextInt64();

            public ThreadLocalFactory(Param param)
            {
                ArgumentNullException.ThrowIfNull(param);
                _param = param;
            }

            public Random Create()
            {
                lock (_sync)
                {
                    if (_block > 127)
                    {
                        _block = 0;
                        _seed = Random.Shared.NextInt64();
                    }

                    var random = new ThreadLocalGenerator(_param, _seed);
                    random.Jump(_block++ * StepBase);
                    return random;
                }
            }
        }

        private sealed class ThreadLocalGenerator(Param param, long seed)
            : LCG64ShiftRandom(param, seed)
        {
            public override void SetSeed(long seed)
            {
                throw new NotSupportedException(
                    "The SetSeed method is not supported for thread-local generators.");
            }
        }
    }

    public sealed class Param : IEquatable<Param>
    {
        public static readonly Param DEFAULT = Of(unchecked((long)0xFBD19FBBC5C07FF5UL), 1);
        public static readonly Param LECUYER1 = Of(unchecked((long)0x27BB2EE687B0B0FDUL), 1);
        public static readonly Param LECUYER2 = Of(unchecked((long)0x2C6FE96EE78B6955UL), 1);
        public static readonly Param LECUYER3 = Of(unchecked((long)0x369DEA0F31A53F85UL), 1);

        private Param(long a, long b)
        {
            A = a;
            B = b;
        }

        public long A { get; }
        public long B { get; }

        public static Param Of(long a, long b)
        {
            return new Param(a, b);
        }

        public bool Equals(Param? other)
        {
            return other is not null && A == other.A && B == other.B;
        }

        public override bool Equals(object? obj)
        {
            return obj is Param other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(A, B);
        }

        public override string ToString()
        {
            return $"{GetType().FullName}[a={A}, b={B}]";
        }
    }
}
