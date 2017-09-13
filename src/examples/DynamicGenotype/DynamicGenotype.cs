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
using System.Linq;
using Jenetics.Engine;
using Jenetics.Internal.Math;
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics.Example
{
    public static class DynamicGenotype
    {
        private static readonly Factory<Genotype<DoubleGene>> Encoding = () =>
        {
            var random = RandomRegistry.GetRandom();
            return Genotype.Of(
                Enumerable.Range(0, random.NextInt(10) + 10)
                    .Select(i => DoubleChromosome.Of(0, 10, random.NextInt(10) + 10))
                    .ToImmutableSeq()
            );
        };

        private static double Fitness(Genotype<DoubleGene> gt)
        {
            Console.WriteLine($"Gene count: {gt.GetNumberOfGenes()}");
            return 0;
        }

        public static void Main()
        {
            var engine = Engine.Engine
                .Builder(Fitness, Encoding)
                .Alterers(new DynamicMutator<DoubleGene, double>(0.25))
                .Build();

            var result = engine.Stream()
                .Take(20)
                .ToBestEvolutionResult();

            Console.WriteLine(result.GetBestFitness());
        }

        private class DynamicMutator<TGene, TAllele> : AltererBase<TGene, TAllele>
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            public DynamicMutator(double probability) : base(probability)
            {
            }

            public override int Alter(Population<TGene, TAllele> population, long generation)
            {
                var p = Math.Pow(Probability, 1.0 / 3.0);
                var alterations = new IntRef(0);

                foreach (var i in random.Indexes(RandomRegistry.GetRandom(), population.Count, p))
                {
                    var pt = population[i];

                    var gt = pt.GetGenotype();
                    var mgt = Mutate(gt, p, alterations);

                    var mpt = pt.NewInstance(mgt, generation);
                    population[i] = mpt;
                }

                return alterations.Value;
            }

            private Genotype<TGene> Mutate(Genotype<TGene> genotype, double p, IntRef alterations)
            {
                IList<IChromosome<TGene>> chromosomes =
                    new List<IChromosome<TGene>>(genotype.ToSeq());

                var random = RandomRegistry.GetRandom();
                var rd = random.NextDouble();
                if (rd < 1 / 3.0)
                    chromosomes.RemoveAt(0);
                else if (rd < 2 / 3.0)
                    chromosomes.Add(chromosomes[0].NewInstance());

                alterations.Value +=
                    Internal.Math.random.Indexes(RandomRegistry.GetRandom(), chromosomes.Count, p)
                        .Select(i => Mutate(chromosomes, i, p))
                        .Sum();

                return Genotype.Of(chromosomes);
            }

            private int Mutate(IList<IChromosome<TGene>> c, int i, double p)
            {
                var chromosome = c[i];
                IList<TGene> genes = new List<TGene>(chromosome.ToSeq());

                var mutations = Mutate(genes, p);
                if (mutations > 0)
                    c[i] = chromosome.NewInstance(ImmutableSeq.Of((IEnumerable<TGene>) genes));
                return mutations;
            }

            private int Mutate(IList<TGene> genes, double p)
            {
                var random = RandomRegistry.GetRandom();

                var rd = random.NextDouble();
                if (rd < 1 / 3.0)
                    genes.RemoveAt(0);
                else if (rd < 2 / 3.0)
                    genes.Add(genes[0].NewInstance());

                return Internal.Math.random.Indexes(random, genes.Count, p)
                    .Select(i => genes[i] = genes[i].NewInstance())
                    .Count();
            }
        }
    }
}