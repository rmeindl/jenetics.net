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
using Jenetics.Util;
using Xunit;

namespace Jenetics.Engine
{
    public class EvolutionResultTest : ObjectTesterBase<EvolutionResult<DoubleGene, double>>
    {
        protected override Factory<EvolutionResult<DoubleGene, double>> Factory()
        {
            return () =>
            {
                var random = RandomRegistry.GetRandom();

                return EvolutionResult.Of(
                    random.NextBoolean() ? Optimize.Maximum : Optimize.Minimum,
                    new Population<DoubleGene, double>(100)
                        .Fill(() => Phenotype.Of(
                                Genotype.Of(DoubleChromosome.Of(0, 1)), 1,
                                a => a.Gene.Allele),
                            100
                        ),
                    random.NextInt(1000),
                    random.NextInt(1000),
                    EvolutionDurations.Of(
                        TimeSpan.FromMilliseconds(random.NextInt(1_000_000)),
                        TimeSpan.FromMilliseconds(random.NextInt(1_000_000)),
                        TimeSpan.FromMilliseconds(random.NextInt(1_000_000)),
                        TimeSpan.FromMilliseconds(random.NextInt(1_000_000)),
                        TimeSpan.FromMilliseconds(random.NextInt(1_000_000)),
                        TimeSpan.FromMilliseconds(random.NextInt(1_000_000)),
                        TimeSpan.FromMilliseconds(random.NextInt(1_000_000))
                    ),
                    random.NextInt(100),
                    random.NextInt(100),
                    random.NextInt(100)
                );
            };
        }

        private static void Shuffle<T>(IList<T> list, Random random)
        {
            var n = list.Count;
            while (n > 1)
            {
                var k = random.Next(0, n) % n;
                n--;
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private static EvolutionResult<IntegerGene, int> NewResult(Optimize opt, int value)
        {
            const int length = 1000;

            int F(Genotype<IntegerGene> gt)
            {
                return gt.Gene.Allele;
            }

            var pop = new Population<IntegerGene, int>(length);
            for (var i = 0; i < length; ++i)
            {
                var gt = Genotype.Of(IntegerChromosome.Of(
                    IntegerGene.Of(value, 0, length)
                ));
                pop.Add(Phenotype.Of(gt, 1, F));
            }

            Shuffle(pop, RandomRegistry.GetRandom());

            return EvolutionResult.Of(opt, pop, 0, 0, EvolutionDurations.Zero, 0, 0, 0);
        }

        [Fact]
        public void BestCollector()
        {
            var bestMaxValue = Enumerable.Range(0, 100)
                .Select(value => NewResult(Optimize.Maximum, value))
                .ToBestEvolutionResult()
                .GetBestFitness();

            Assert.Equal(99, bestMaxValue);

            var bestMinValue = Enumerable.Range(0, 100)
                .Select(value => NewResult(Optimize.Minimum, value))
                .ToBestGenotype()
                .Gene.Allele;

            Assert.Equal(0, bestMinValue);
        }

        [Fact]
        public void BestWorstPhenotype()
        {
            const int length = 100;

            int F(Genotype<IntegerGene> gt)
            {
                return gt.Gene.Allele;
            }

            var population = new Population<IntegerGene, int>(length);
            for (var i = 0; i < length; ++i)
            {
                var gt = Genotype.Of(IntegerChromosome.Of(
                    IntegerGene.Of(i, 0, length)
                ));
                population.Add(Phenotype.Of(gt, 1, F));
            }

            Shuffle(population, RandomRegistry.GetRandom());

            var maxResult = EvolutionResult.Of(
                Optimize.Maximum, population,
                0, 0, EvolutionDurations.Zero, 0, 0, 0
            );

            Assert.Equal(length - 1, maxResult.GetBestFitness());
            Assert.Equal(0, maxResult.GetWorstFitness());

            var minResult = EvolutionResult.Of(
                Optimize.Minimum, population,
                0, 0, EvolutionDurations.Zero, 0, 0, 0
            );

            Assert.Equal(0, minResult.GetBestFitness());
            Assert.Equal(length - 1, minResult.GetWorstFitness());
        }

        [Fact]
        public void CompareTo()
        {
            const int length = 100;

            int F(Genotype<IntegerGene> gt)
            {
                return gt.Gene.Allele;
            }

            var small = new Population<IntegerGene, int>(length);
            for (var i = 0; i < length; ++i)
            {
                var gt = Genotype.Of(IntegerChromosome.Of(
                    IntegerGene.Of(i, 0, length)
                ));
                small.Add(Phenotype.Of(gt, 1, F));
            }
            Shuffle(small, RandomRegistry.GetRandom());

            var big = new Population<IntegerGene, int>(length);
            for (var i = 0; i < length; ++i)
            {
                var gt = Genotype.Of(IntegerChromosome.Of(
                    IntegerGene.Of(i + length, 0, length)
                ));
                big.Add(Phenotype.Of(gt, 1, F));
            }
            Shuffle(big, RandomRegistry.GetRandom());


            var smallMaxResult = EvolutionResult.Of(
                Optimize.Maximum, small,
                0, 0, EvolutionDurations.Zero, 0, 0, 0
            );
            var bigMaxResult = EvolutionResult.Of(
                Optimize.Maximum, big,
                0, 0, EvolutionDurations.Zero, 0, 0, 0
            );

            Assert.True(smallMaxResult.CompareTo(bigMaxResult) < 0);
            Assert.True(bigMaxResult.CompareTo(smallMaxResult) > 0);
            Assert.True(smallMaxResult.CompareTo(smallMaxResult) == 0);
            Assert.True(bigMaxResult.CompareTo(bigMaxResult) == 0);


            var smallMinResult = EvolutionResult.Of(
                Optimize.Minimum, small,
                0, 0, EvolutionDurations.Zero, 0, 0, 0
            );
            var bigMinResult = EvolutionResult.Of(
                Optimize.Minimum, big,
                0, 0, EvolutionDurations.Zero, 0, 0, 0
            );

            Assert.True(smallMinResult.CompareTo(bigMinResult) > 0);
            Assert.True(bigMinResult.CompareTo(smallMinResult) < 0);
            Assert.True(smallMinResult.CompareTo(smallMinResult) == 0);
            Assert.True(bigMinResult.CompareTo(bigMinResult) == 0);
        }

        [Fact]
        public void EmptyStreamCollectEvolutionResult()
        {
            var engine = Engine
                .Builder(a => a.Gene.Allele, DoubleChromosome.Of(0, 1))
                .Build();

            var result = engine.Stream()
                .Take(0)
                .ToBestEvolutionResult();

            Assert.Null(result);
        }

        [Fact]
        public void EmptyStreamCollectGenotype()
        {
            var engine = Engine
                .Builder(a => a.Gene.Allele, DoubleChromosome.Of(0, 1))
                .Build();

            var result = engine.Stream()
                .Take(0)
                .ToBestGenotype();

            Assert.Null(result);
        }

        [Fact]
        public void EmptyStreamCollectPhenotype()
        {
            var engine = Engine
                .Builder(a => a.Gene.Allele, DoubleChromosome.Of(0, 1))
                .Build();

            var result = engine.Stream()
                .Take(0)
                .ToBestPhenotype();

            Assert.Null(result);
        }
    }
}