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

namespace Jenetics
{
    public class RouletteWheelSelector<TGene, TAllele> : ProbabilitySelectorBase<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        public RouletteWheelSelector() : this(false)
        {
        }

        protected RouletteWheelSelector(bool sorted) : base(sorted)
        {
        }

        public override double[] Probabilities(Population<TGene, TAllele> population, int count)
        {
            var fitness = new double[population.Count];
            for (var i = population.Count; --i >= 0;)
                fitness[i] = population[i].GetFitness().ToDouble(null);

            var worst = Math.Min(Statistics.Min(fitness), 0.0);
            var sum = DoubleAdder.Sum(fitness) - worst * population.Count;

            if (Equality.Eq(sum, 0.0))
                for (var i = 0; i < fitness.Length; i++)
                    fitness[i] = 1.0 / population.Count;
            else
                for (var i = population.Count; --i >= 0;)
                    fitness[i] = (fitness[i] - worst) / sum;

            return fitness;
        }

        public override bool Equals(object obj)
        {
            return Equality.OfType(this, obj);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).Value;
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}