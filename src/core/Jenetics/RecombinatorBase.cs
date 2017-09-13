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
using System.Linq;
using Jenetics.Util;
using static Jenetics.Internal.Math.Base;

namespace Jenetics
{
    public abstract class RecombinatorBase<TGene, TAllele> : AltererBase<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        protected RecombinatorBase(double probability, int order) : base(probability)
        {
            if (order < 2)
                throw new ArgumentOutOfRangeException($"Order must be greater than one, but was {order}.");
            Order = order;
        }

        public int Order { get; }

        public override int Alter(Population<TGene, TAllele> population, long generation)
        {
            var count = 0;
            if (population.Count >= 2)
            {
                var random = RandomRegistry.GetRandom();
                var order = Math.Min(Order, population.Count);

                int[] Individuals(int i)
                {
                    var ind = Subset(population.Count, order, random);
                    ind[0] = i;
                    return ind;
                }

                count = Internal.Math.random.Indexes(random, population.Count, Probability)
                    .Select(Individuals).Select(i => Recombine(population, i, generation)).Sum();
            }

            return count;
        }

        protected internal abstract int Recombine(Population<TGene, TAllele> population, int[] individuals,
            long generation);
    }
}