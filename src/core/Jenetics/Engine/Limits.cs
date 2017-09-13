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
using System.Threading;

namespace Jenetics.Engine
{
    public static class Limits
    {
        public static Func<EvolutionResult<TGene, TAllele>, bool> BySteadyFitness<TGene, TAllele>(int generations)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return new SteadyFitnessLimit<TAllele>(generations).Test;
        }

        public static Func<EvolutionResult<TGene, TAllele>, bool> ByFixedGeneration<TGene, TAllele>(long generation)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
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
    }
}