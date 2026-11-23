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
using System.Globalization;
using System.Threading;
using Jenetics.Stat;

namespace Jenetics.Engine;

public static class Limits
{
    public static Func<EvolutionResult<TGene, TAllele>, bool> BySteadyFitness<TGene, TAllele>(int generations)
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        return new SteadyFitnessLimit<TAllele>(generations).Test;
    }

    public static Func<EvolutionResult<TGene, TAllele>, bool> ByExecutionTime<TGene, TAllele>(
        TimeSpan duration,
        TimeProvider timeProvider
    )
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        return new ExecutionTimeLimit(duration, timeProvider).Test;
    }

    public static Func<EvolutionResult<TGene, TAllele>, bool> ByExecutionTime<TGene, TAllele>(
        TimeSpan duration
    )
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        return ByExecutionTime<TGene, TAllele>(duration, TimeProvider.System);
    }

    public static Func<EvolutionResult<TGene, TAllele>, bool> ByFitnessThreshold<TGene, TAllele>(
        TAllele threshold
    )
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        ArgumentNullException.ThrowIfNull(threshold);
        return result => result.GetBestFitness() is { } bestFitness &&
                         result.GetOptimize().Compare(bestFitness, threshold) < 0;
    }

    public static Func<EvolutionResult<TGene, TAllele>, bool> ByFitnessConvergence<TGene, TAllele>(
        int shortFilterSize,
        int longFilterSize,
        Func<DoubleMoments, DoubleMoments, bool> proceed
    )
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        return new FitnessConvergenceLimit<TAllele>(
            shortFilterSize,
            longFilterSize,
            proceed
        ).Test<TGene>;
    }

    public static Func<EvolutionResult<TGene, TAllele>, bool> ByFitnessConvergence<TGene, TAllele>(
        int shortFilterSize,
        int longFilterSize,
        double epsilon
    )
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        if (epsilon < 0.0 || epsilon > 1.0)
            throw new ArgumentOutOfRangeException(nameof(epsilon), "Epsilon must be in [0, 1].");

        return ByFitnessConvergence<TGene, TAllele>(
            shortFilterSize,
            longFilterSize,
            (shortMoments, longMoments) =>
                RelativeDifference(shortMoments.Mean, longMoments.Mean) >= epsilon
        );
    }

    public static Func<EvolutionResult<TGene, TAllele>, bool> ByPopulationConvergence<TGene, TAllele>(
        Func<double, DoubleMoments, bool> proceed
    )
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        ArgumentNullException.ThrowIfNull(proceed);
        return result =>
        {
            var fitness = new DoubleMomentStatistics();
            foreach (var phenotype in result.GetPopulation())
            {
                if (phenotype.GetFitness() is { } value)
                    fitness.Accept(Convert.ToDouble(value, CultureInfo.InvariantCulture));
            }

            var best = result.GetBestFitness();
            var bestFitness = best is null
                ? double.NaN
                : Convert.ToDouble(best, CultureInfo.InvariantCulture);

            return proceed(bestFitness, DoubleMoments.Of(fitness));
        };
    }

    public static Func<EvolutionResult<TGene, TAllele>, bool> ByPopulationConvergence<TGene, TAllele>(
        double epsilon
    )
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        if (epsilon < 0.0 || epsilon > 1.0)
            throw new ArgumentOutOfRangeException(nameof(epsilon), "Epsilon must be in [0, 1].");

        return ByPopulationConvergence<TGene, TAllele>(
            (best, moments) => RelativeDifference(best, moments.Mean) >= epsilon
        );
    }

    public static Func<EvolutionResult<TGene, TAllele>, bool> ByFixedGeneration<TGene, TAllele>(long generation)
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        if (generation < 0)
            throw new ArgumentException(
                $"The number of generations must be greater or equal then zero, but was {generation}");

        return new FixedGenerationLimit(generation).Test;
    }

    private class FixedGenerationLimit
    {
        private readonly long _generation;
        private long _current;

        internal FixedGenerationLimit(long generation)
        {
            _current = 0;
            _generation = generation;
        }

        public bool Test(object result)
        {
            return Interlocked.Increment(ref _current) <= _generation;
        }
    }

    private static double RelativeDifference(double a, double b)
    {
        var divisor = Math.Max(Math.Abs(a), Math.Abs(b));
        return Math.Abs(a - b) / (divisor <= 10E-20 ? 1.0 : divisor);
    }

    private sealed class ExecutionTimeLimit
    {
        private readonly TimeSpan _duration;
        private readonly TimeProvider _timeProvider;
        private long _start = long.MinValue;

        public ExecutionTimeLimit(TimeSpan duration, TimeProvider timeProvider)
        {
            _duration = duration;
            _timeProvider = timeProvider;
        }

        public bool Test<TGene, TAllele>(EvolutionResult<TGene, TAllele> result)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>
        {
            var timestamp = _timeProvider.GetTimestamp();
            Interlocked.CompareExchange(ref _start, timestamp, long.MinValue);
            return _timeProvider.GetElapsedTime(Interlocked.Read(ref _start), timestamp) < _duration;
        }
    }

    private sealed class FitnessConvergenceLimit<TAllele>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly int _shortFilterSize;
        private readonly int _longFilterSize;
        private readonly double[] _buffer;
        private readonly Func<DoubleMoments, DoubleMoments, bool> _proceed;
        private int _position;
        private int _length;

        public FitnessConvergenceLimit(
            int shortFilterSize,
            int longFilterSize,
            Func<DoubleMoments, DoubleMoments, bool> proceed
        )
        {
            if (shortFilterSize < 1)
                throw new ArgumentOutOfRangeException(nameof(shortFilterSize));
            if (longFilterSize < 2)
                throw new ArgumentOutOfRangeException(nameof(longFilterSize));
            if (shortFilterSize >= longFilterSize)
                throw new ArgumentException("The long filter size must exceed the short filter size.");

            ArgumentNullException.ThrowIfNull(proceed);
            _shortFilterSize = shortFilterSize;
            _longFilterSize = longFilterSize;
            _buffer = new double[longFilterSize];
            _proceed = proceed;
        }

        public bool Test<TGene>(EvolutionResult<TGene, TAllele> result)
            where TGene : IGene<TGene>
        {
            var fitness = result.GetBestFitness();
            if (fitness is not null)
            {
                _buffer[_position] = Convert.ToDouble(fitness, CultureInfo.InvariantCulture);
                _position = (_position + 1) % _buffer.Length;
                _length = Math.Min(_length + 1, _buffer.Length);
            }

            return _length < _buffer.Length ||
                   _proceed(GetMoments(_shortFilterSize), GetMoments(_longFilterSize));
        }

        private DoubleMoments GetMoments(int windowSize)
        {
            var length = Math.Min(windowSize, _length);
            var statistics = new DoubleMomentStatistics();
            var start = (_position + _buffer.Length - length) % _buffer.Length;
            for (var i = 0; i < length; ++i)
                statistics.Accept(_buffer[(start + i) % _buffer.Length]);
            return DoubleMoments.Of(statistics);
        }
    }
}