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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jenetics.Util;

namespace Jenetics
{
    public static class TestUtils
    {
        internal static readonly Func<Genotype<DoubleGene>, double> Ff = gt => gt.Gene.Allele;

        public static Population<DoubleGene, double> NewDoubleGenePopulation(
            int ngenes,
            int nchromosomes,
            int npopulation
        )
        {
            var chromosomes = MutableSeq.OfLength<IChromosome<DoubleGene>>(nchromosomes);

            for (var i = 0; i < nchromosomes; ++i)
                chromosomes[i] = DoubleChromosome.Of(0, 10, ngenes);

            var genotype = new Genotype<DoubleGene>(chromosomes.ToImmutableSeq());
            var population = new Population<DoubleGene, double>(npopulation);

            for (var i = 0; i < npopulation; ++i)
                population.Add(Phenotype.Of(genotype.NewInstance(), 0, Ff).Evaluate());

            return population;
        }


        public static Population<DoubleGene, double> NewDoublePopulation(int length)
        {
            return NewDoublePopulation(length, 0, 10);
        }

        public static Population<DoubleGene, double> NewDoublePopulation(int length, double min, double max)
        {
            var population = new Population<DoubleGene, double>(length);

            for (var i = 0; i < length; ++i)
                population.Add(NewDoublePhenotype(min, max));

            return population;
        }

        public static Phenotype<DoubleGene, double> NewDoublePhenotype(double value)
        {
            return Phenotype.Of(Genotype.Of(
                    DoubleChromosome.Of(DoubleGene.Of(value, 0, 10))), 0, Ff
            ).Evaluate();
        }

        public static Phenotype<DoubleGene, double> NewDoublePhenotype(double min, double max)
        {
            var random = RandomRegistry.GetRandom();
            return NewDoublePhenotype(random.NextDouble() * (max - min) + min);
        }

        public static Population<EnumGene<double>, double> NewPermutationDoubleGenePopulation(int ngenes,
            int nchromosomes, int npopulation)
        {
            var random = new Random(122343);
            var alleles = MutableSeq.OfLength<double>(ngenes);
            for (var i = 0; i < ngenes; ++i)
                alleles[i] = random.NextDouble() * 10;
            var ialleles = alleles.ToImmutableSeq();

            var chromosomes = MutableSeq.OfLength<PermutationChromosome<double>>(nchromosomes);

            for (var i = 0; i < nchromosomes; ++i)
                chromosomes[i] = PermutationChromosome.Of(ialleles);

            var genotype =
                new Genotype<EnumGene<double>>(chromosomes.Cast<IChromosome<EnumGene<double>>>().ToImmutableSeq());
            var population = new Population<EnumGene<double>, double>(npopulation);

            for (var i = 0; i < npopulation; ++i)
                population.Add(Phenotype.Of(genotype.NewInstance(), 0, gt => gt.Gene.Allele));

            return population;
        }

        public static int Diff(Population<DoubleGene, double> p1, Population<DoubleGene, double> p2)
        {
            var count = 0;
            for (var i = 0; i < p1.Count; ++i)
            {
                var gt1 = p1[i].GetGenotype();
                var gt2 = p2[i].GetGenotype();

                for (var j = 0; j < gt1.Length; ++j)
                {
                    var c1 = gt1.GetChromosome(j);
                    var c2 = gt2.GetChromosome(j);

                    for (var k = 0; k < c1.Length; ++k)
                        if (!c1.GetGene(k).Equals(c2.GetGene(k)))
                            ++count;
                }
            }
            return count;
        }

        public class AlterCountParameters : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] {1, 1, 100},
                new object[] {5, 1, 100},
                new object[] {80, 1, 100},
                new object[] {1, 2, 100},
                new object[] {5, 2, 100},
                new object[] {80, 2, 100},
                new object[] {1, 15, 100},
                new object[] {5, 15, 100},
                new object[] {80, 15, 100},

                new object[] {1, 1, 150},
                new object[] {5, 1, 150},
                new object[] {80, 1, 150},
                new object[] {1, 2, 150},
                new object[] {5, 2, 150},
                new object[] {80, 2, 150},
                new object[] {1, 15, 150},
                new object[] {5, 15, 150},
                new object[] {80, 15, 150},

                new object[] {1, 1, 500},
                new object[] {5, 1, 500},
                new object[] {80, 1, 500},
                new object[] {1, 2, 500},
                new object[] {5, 2, 500},
                new object[] {80, 2, 500},
                new object[] {1, 15, 500},
                new object[] {5, 15, 500},
                new object[] {80, 15, 500}
            };

            public IEnumerator<object[]> GetEnumerator()
            {
                return _data.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class AlterProbabilityParameters : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] {20, 20, 20, 0.5},
                new object[] {1, 1, 150, 0.15},
                new object[] {5, 1, 150, 0.15},
                new object[] {80, 1, 150, 0.15},
                new object[] {1, 2, 150, 0.15},
                new object[] {5, 2, 150, 0.15},
                new object[] {80, 2, 150, 0.15},
                new object[] {1, 15, 150, 0.15},
                new object[] {5, 15, 150, 0.15},
                new object[] {80, 15, 150, 0.15},

                new object[] {1, 1, 150, 0.5},
                new object[] {5, 1, 150, 0.5},
                new object[] {80, 1, 150, 0.5},
                new object[] {1, 2, 150, 0.5},
                new object[] {5, 2, 150, 0.5},
                new object[] {80, 2, 150, 0.5},
                new object[] {1, 15, 150, 0.5},
                new object[] {5, 15, 150, 0.5},
                new object[] {80, 15, 150, 0.5},

                new object[] {1, 1, 150, 0.85},
                new object[] {5, 1, 150, 0.85},
                new object[] {80, 1, 150, 0.85},
                new object[] {1, 2, 150, 0.85},
                new object[] {5, 2, 150, 0.85},
                new object[] {80, 2, 150, 0.85},
                new object[] {1, 15, 150, 0.85},
                new object[] {5, 15, 150, 0.85},
                new object[] {80, 15, 150, 0.85}
            };

            public IEnumerator<object[]> GetEnumerator()
            {
                return _data.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}