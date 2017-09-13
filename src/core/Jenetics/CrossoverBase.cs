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
using Jenetics.Util;

namespace Jenetics
{
    public abstract class CrossoverBase<TGene, TAllele> : RecombinatorBase<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        protected CrossoverBase(double probability) : base(probability, 2)
        {
        }

        protected internal override int Recombine(Population<TGene, TAllele> population, int[] individuals,
            long generation)
        {
            var random = RandomRegistry.GetRandom();

            var pt1 = population[individuals[0]];
            var pt2 = population[individuals[1]];
            var gt1 = pt1.GetGenotype();
            var gt2 = pt2.GetGenotype();

            var chIndex = random.NextInt(Math.Min(gt1.Length, gt2.Length));

            var c1 = MutableSeq.Of<IChromosome<TGene>>(gt1.ToSeq());
            var c2 = MutableSeq.Of<IChromosome<TGene>>(gt2.ToSeq());
            var genes1 = MutableSeq.Of<TGene>(c1[chIndex].ToSeq());
            var genes2 = MutableSeq.Of<TGene>(c2[chIndex].ToSeq());

            Crossover(genes1, genes2);

            c1[chIndex] = c1[chIndex].NewInstance(genes1.ToImmutableSeq());
            c2[chIndex] = c2[chIndex].NewInstance(genes2.ToImmutableSeq());

            population[individuals[0]] = pt1.NewInstance(gt1.NewInstance(c1.ToImmutableSeq()), generation);
            population[individuals[1]] = pt2.NewInstance(gt1.NewInstance(c2.ToImmutableSeq()), generation);

            return Order;
        }

        protected internal abstract int Crossover(IMutableSeq<TGene> that, IMutableSeq<TGene> other);
    }
}