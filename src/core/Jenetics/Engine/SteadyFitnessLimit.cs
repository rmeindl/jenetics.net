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

namespace Jenetics.Engine
{
    internal class SteadyFitnessLimit<TAllele>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly int _generations;
        private TAllele _fitness;

        private bool _proceed = true;
        private int _stable;

        public SteadyFitnessLimit(int generations)
        {
            if (generations < 1)
                throw new ArgumentException("Generations < 1: " + generations);
            _generations = generations;
        }

        public bool Test<T>(EvolutionResult<T, TAllele> result)
            where T : IGene<T>
        {
            if (!_proceed) return false;

            if (_fitness == null)
            {
                _fitness = result.GetBestFitness();
                _stable = 1;
            }
            else
            {
                var opt = result.GetOptimize();
                if (opt.Compare(_fitness, result.GetBestFitness()) >= 0)
                {
                    _proceed = ++_stable <= _generations;
                }
                else
                {
                    _fitness = result.GetBestFitness();
                    _stable = 1;
                }
            }

            return _proceed;
        }
    }
}