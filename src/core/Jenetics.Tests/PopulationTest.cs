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
using Xunit;

namespace Jenetics
{
    public class PopulationTest : ObjectTesterBase<Population<DoubleGene, double>>
    {
        private static readonly Func<Genotype<DoubleGene>, double> Ff = gt => gt.Gene.Allele;

        private static readonly Func<double, double> Fs = d => d;

        private static Phenotype<DoubleGene, double> Pt(double value)
        {
            return Phenotype.Of(Genotype.Of(DoubleChromosome.Of(DoubleGene.Of(value, 0, 10))), 0, Ff, Fs);
        }

        protected override Factory<Population<DoubleGene, double>> Factory()
        {
            return () =>
            {
                var gt = Genotype.Of(DoubleChromosome.Of(0, 1));

                return new Population<DoubleGene, double>(100)
                    .Fill(() => Phenotype.Of(gt.NewInstance(), 1, Ff, Fs), 100);
            };
        }

        [Fact]
        public void Empty()
        {
            var genotype = Genotype.Of(
                Enumerable.Range(0, 10)
                    .Select(i => DoubleChromosome.Of(0, 10, 10))
                    .ToImmutableSeq()
            );

            var pop = Population.Empty<DoubleGene, bool>();

            Assert.True(0 == pop.Count);
            Assert.True(pop.IsEmpty);

            pop.Add(Phenotype.Of(genotype, 1, chromosomes => true));

            Assert.True(1 == pop.Count);
            Assert.False(pop.IsEmpty);
        }

        [Fact]
        public void Sort()
        {
            var population = new Population<DoubleGene, double>();
            var random = RandomRegistry.GetRandom();
            for (var i = 0; i < 100; ++i)
                population.Add(Pt(random.Next() * 9.0));

            population.PopulationSort();
            for (var i = 0; i < population.Count - 1; ++i)
            {
                var first = Ff(population[i].GetGenotype());
                var second = Ff(population[i + 1].GetGenotype());

                Assert.True(first.CompareTo(second) >= 0);
            }

            Lists.Shuffle(population);
            population.SortWith(Optimize.Maximum.Descending<double>());
            for (var i = 0; i < population.Count - 1; ++i)
            {
                var first = Ff(population[i].GetGenotype());
                var second = Ff(population[i + 1].GetGenotype());

                Assert.True(first.CompareTo(second) >= 0, first + "<" + second);
            }

            Lists.Shuffle(population);
            population.SortWith(Optimize.Minimum.Descending<double>());
            for (var i = 0; i < population.Count - 1; ++i)
            {
                var first = Ff(population[i].GetGenotype());
                var second = Ff(population[i + 1].GetGenotype());

                Assert.True(first.CompareTo(second) <= 0, first + ">" + second);
            }
        }
    }
}