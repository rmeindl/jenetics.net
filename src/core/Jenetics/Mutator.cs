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
using Jenetics.Internal.Util;
using Jenetics.Util;
using static Jenetics.Internal.Math.random;

namespace Jenetics
{
    public class Mutator<TGene, TAllele> : AltererBase<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        public Mutator() : this(0.01)
        {
        }

        public Mutator(double probability) : base(probability)
        {
        }

        public override int Alter(Population<TGene, TAllele> population, long generation)
        {
            var p = Math.Pow(Probability, 1.0 / 3.0);
            var alterations = new IntRef(0);

            foreach (var i in Indexes(RandomRegistry.GetRandom(), population.Count, p))
            {
                var pt = population[i];

                var gt = pt.GetGenotype();
                var mgt = Mutate(gt, p, alterations);

                var mpt = pt.NewInstance(mgt, generation);
                population[i] = mpt;
            }

            return alterations.Value;
        }

        protected int Mutate(IMutableSeq<IChromosome<TGene>> c, int i, double p)
        {
            var chromosome = c[i];
            var genes = MutableSeq.Of<TGene>(chromosome.ToSeq());

            var mutations = Mutate(genes, p);
            if (mutations > 0)
                c[i] = chromosome.NewInstance(genes.ToImmutableSeq());
            return mutations;
        }

        protected virtual int Mutate(IMutableSeq<TGene> genes, double p)
        {
            return Indexes(RandomRegistry.GetRandom(), genes.Length, p)
                .Select(i => genes[i] = genes[i].NewInstance())
                .Count();
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
            return $"{GetType().Name}[p={Probability}]";
        }

        private Genotype<TGene> Mutate(Genotype<TGene> genotype, double p, IntRef alterations)
        {
            var chromosomes = genotype.ToSeq().Copy();

            alterations.Value += Indexes(RandomRegistry.GetRandom(), genotype.Length, p)
                .Select(i => Mutate(chromosomes, i, p)).Sum();

            return genotype.NewInstance(chromosomes.ToImmutableSeq());
        }
    }
}