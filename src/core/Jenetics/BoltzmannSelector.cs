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
using System.Collections.Generic;
using Jenetics.Internal.Math;
using Jenetics.Internal.Util;

namespace Jenetics
{
    public class BoltzmannSelector<TGene, TAllele> : ProbabilitySelectorBase<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly double _b;


        public BoltzmannSelector(double b)
        {
            _b = b;
        }

        public BoltzmannSelector() : this(4.0)
        {
        }

        public override double[] Probabilities(Population<TGene, TAllele> population, int count)
        {
            var fitness = new double[population.Count];

            // Copy the fitness values to probabilities arrays.
            fitness[0] = population[0].GetFitness().ToDouble(null);

            var min = fitness[0];
            var max = fitness[0];
            for (var i = 1; i < fitness.Length; ++i)
            {
                fitness[i] = population[i].GetFitness().ToDouble(null);
                if (fitness[i] < min) min = fitness[i];
                else if (fitness[i] > max) max = fitness[i];
            }

            var diff = max - min;
            if (ProbabilitySelector.Eq(diff, 0.0))
            {
                // Set equal probabilities if diff (almost) zero.
                Array.Fill(fitness, 1.0 / fitness.Length);
            }
            else
            {
                // Scale fitness values to avoid overflow.
                for (var i = fitness.Length; --i >= 0;)
                    fitness[i] = (fitness[i] - min) / diff;

                // Apply the "Boltzmann" function.
                for (var i = fitness.Length; --i >= 0;)
                    fitness[i] = Math.Exp(_b * fitness[i]);
            }

            return Arithmetics.Normalize(fitness);
        }

        public override bool Equals(object obj)
        {
            return obj is BoltzmannSelector<TGene, TAllele> selector &&
                   Comparer<double>.Default.Compare(_b, selector._b) == 0;
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(_b).Value;
        }

        public override string ToString()
        {
            return $"BoltzmannSelector[b={_b}]";
        }
    }
}