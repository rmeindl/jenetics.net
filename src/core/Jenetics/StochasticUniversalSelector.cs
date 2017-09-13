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
using Jenetics.Internal.Util;
using Jenetics.Util;
using static Jenetics.Internal.Util.Require;

namespace Jenetics
{
    public class StochasticUniversalSelector<TGene, TAllele> : RouletteWheelSelector<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        public StochasticUniversalSelector() : base(true)
        {
        }


        public override Population<TGene, TAllele> Select(
            Population<TGene, TAllele> population,
            int count,
            Optimize opt
        )
        {
            NonNull(population, "Population");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count),
                    $"Selection count must be greater or equal then zero, but was {count}");

            var selection = new Population<TGene, TAllele>(count);
            if (count == 0 || population.IsEmpty)
                return selection;

            var pop = Copy(population);
            var probabilities = base.Probabilities(pop, count, opt);

            var delta = 1.0 / count;
            var points = new double[count];
            points[0] = RandomRegistry.GetRandom().NextDouble() * delta;
            for (var i = 1; i < count; ++i)
                points[i] = delta * i;

            var j = 0;
            double prop = 0;
            for (var i = 0; i < count; ++i)
            {
                while (points[i] > prop)
                {
                    prop += probabilities[j];
                    ++j;
                }
                selection.Add(pop[j % pop.Count]);
            }

            return selection;
        }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(base.Equals);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(base.GetHashCode()).Value;
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}