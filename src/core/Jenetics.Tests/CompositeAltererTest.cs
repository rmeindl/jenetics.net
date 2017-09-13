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
using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public class CompositeAltererTest
    {
        public IAlterer<DoubleGene, double> NewAlterer(double p)
        {
            var p3 = Math.Pow(p, 3);
            return CompositeAlterer.Of(
                new Mutator<DoubleGene, double>(p3),
                new Mutator<DoubleGene, double>(p3),
                new Mutator<DoubleGene, double>(p3)
            );
        }

        [Theory]
        [MemberData(nameof(AlterCountParameters))]
        public void AlterCount(int ngenes, int nchromosomes, int npopulation)
        {
            var p1 = Population(
                ngenes, nchromosomes, npopulation
            );
            var p2 = p1.Copy();
            Assert.Equal(p1, p2);

            var mutator = NewAlterer(0.01);

            var mutated = mutator.Alter(p1, 1);

            Assert.Equal(Diff(p1, p2), mutated);
        }

        public static Population<DoubleGene, double> Population(int ngenes, int nchromosomes, int npopulation)
        {
            var chromosomes = MutableSeq.OfLength<IChromosome<DoubleGene>>(nchromosomes);

            for (var i = 0; i < nchromosomes; ++i)
                chromosomes[i] = DoubleChromosome.Of(0, 10, ngenes);

            var genotype = new Genotype<DoubleGene>(chromosomes.ToImmutableSeq());
            var population = new Population<DoubleGene, double>(npopulation);

            for (var i = 0; i < npopulation; ++i)
                population.Add(Phenotype.Of(genotype.NewInstance(), 0, TestUtils.Ff));

            return population;
        }

        public int Diff(Population<DoubleGene, double> p1, Population<DoubleGene, double> p2)
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

        public static IEnumerable<object[]> AlterCountParameters()
        {
            return new List<object[]>
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
        }

        [Fact]
        public void Join()
        {
            var alterer = CompositeAlterer.Join(
                new Mutator<DoubleGene, double>(),
                new SwapMutator<DoubleGene, double>()
            );

            Assert.Equal(2, alterer.Alterers.Length);
            Assert.Equal(new Mutator<DoubleGene, double>(), alterer.Alterers[0]);
            Assert.Equal(new SwapMutator<DoubleGene, double>(), alterer.Alterers[1]);

            alterer = CompositeAlterer.Join(alterer, new MeanAlterer<DoubleGene, double>());

            Assert.Equal(3, alterer.Alterers.Length);
            Assert.Equal(new Mutator<DoubleGene, double>(), alterer.Alterers[0]);
            Assert.Equal(new SwapMutator<DoubleGene, double>(), alterer.Alterers[1]);
            Assert.Equal(new MeanAlterer<DoubleGene, double>(), alterer.Alterers[2]);

            alterer = CompositeAlterer.Of(
                new MeanAlterer<DoubleGene, double>(),
                new SwapMutator<DoubleGene, double>(),
                alterer,
                new SwapMutator<DoubleGene, double>()
            );

            Assert.Equal(6, alterer.Alterers.Length);
            Assert.Equal(new MeanAlterer<DoubleGene, double>(), alterer.Alterers[0]);
            Assert.Equal(new SwapMutator<DoubleGene, double>(), alterer.Alterers[1]);
            Assert.Equal(new Mutator<DoubleGene, double>(), alterer.Alterers[2]);
            Assert.Equal(new SwapMutator<DoubleGene, double>(), alterer.Alterers[3]);
            Assert.Equal(new MeanAlterer<DoubleGene, double>(), alterer.Alterers[4]);
            Assert.Equal(new SwapMutator<DoubleGene, double>(), alterer.Alterers[5]);
        }
    }
}