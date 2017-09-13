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
using Jenetics.Stat;
using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public abstract class SelectorTesterBase<T> : ObjectTesterBase<T>
        where T : class, ISelector<DoubleGene, double>
    {
        private const int ClassCount = 25;
        private const double Min = 0.0;
        private const double Max = 1_000.0;
        private const double SelectionFraction = 2.0;
        protected const int PopulationCount = (int) (ClassCount * 10 * SelectionFraction);

        protected virtual T Selector()
        {
            return Factory()();
        }


        [Theory]
        [MemberData(nameof(SelectionPerformanceParameters))]
        public void SelectionPerformance(int size, int count, Optimize opt)
        {
            Func<Genotype<DoubleGene>, double> ff = g => g.Gene.Allele;

            Phenotype<DoubleGene, double> F()
            {
                return Phenotype.Of(Genotype.Of(DoubleChromosome.Of(0.0, 100.0)), 1, ff);
            }

            RandomRegistry.Using(new Random(543455), r =>
            {
                var population = Enumerable.Range(0, size)
                    .Select(i => F())
                    .ToPopulation();

                ISelector<DoubleGene, double> selector = Selector();

                if (!(selector is MonteCarloSelector<DoubleGene, double>))
                {
                    var monteCarloSelectionSum =
                        new MonteCarloSelector<DoubleGene, double>()
                            .Select(population, count, opt)
                            .Select(p => p.GetFitness())
                            .Sum();

                    var selectionSum =
                        selector
                            .Select(population, count, opt)
                            .Select(p => p.GetFitness())
                            .Sum();

                    if (opt == Optimize.Maximum)
                        Assert.True(
                            selectionSum > monteCarloSelectionSum,
                            $"{selectionSum} <= {monteCarloSelectionSum}");
                    else
                        Assert.True(
                            selectionSum < monteCarloSelectionSum,
                            $"{selectionSum} >= {monteCarloSelectionSum}");
                }
            });
        }


        [Theory]
        [MemberData(nameof(SelectParameters))]
        public void Select(int size, int count, Optimize opt)
        {
            Func<Genotype<DoubleGene>, double> ff = gt => gt.Gene.Allele;

            Phenotype<DoubleGene, double> F()
            {
                return Phenotype.Of(Genotype.Of(DoubleChromosome.Of(0.0, 1_000.0)), 1, ff);
            }

            var population = Enumerable.Range(0, size)
                .Select(i => F())
                .ToPopulation();

            var selection = Selector().Select(population, count, opt);

            if (size == 0)
                Assert.Empty(selection);
            else
                Assert.Equal(count, selection.Count);
            foreach (var pt in selection)
                Assert.True(
                    population.Contains(pt),
                    $"Population doesn't contain {pt}."
                );
        }

        protected Histogram<double> Distribution(
            ISelector<DoubleGene, double> selector,
            Optimize opt,
            int populationCount,
            int loops
        )
        {
            Func<Genotype<DoubleGene>, double> ff = gt => gt.Gene.Allele;

            Factory<Phenotype<DoubleGene, double>> ptf = () =>
                Phenotype.Of(Genotype.Of(DoubleChromosome.Of(Min, Max)), 1, ff);

            return Enumerable.Range(0, loops).AsParallel().Select(j =>
            {
                var hist = Histogram.OfDouble(Min, Max, ClassCount);

                var population =
                    Enumerable.Range(0, populationCount)
                        .Select(i => ptf())
                        .ToPopulation();

                var selectionCount = (int) (populationCount / SelectionFraction);
                foreach (var pt in selector.Select(population, selectionCount, opt)
                    .Select(pt => pt.GetGenotype().Gene.Allele))
                    hist.Accept(pt);

                return hist;
            }).ToDoubleHistogram(Min, Max, ClassCount);
        }

        public static IEnumerable<object[]> SelectionPerformanceParameters()
        {
            yield return new object[] {200, 100, Optimize.Maximum};
            yield return new object[] {2000, 1000, Optimize.Minimum};
            yield return new object[] {200, 100, Optimize.Maximum};
            yield return new object[] {2000, 1000, Optimize.Minimum};
        }

        public static IEnumerable<object[]> SelectParameters()
        {
            IList<int> sizes = new List<int> {0, 1, 2, 3, 5, 11, 50, 100, 10_000};
            IList<int> counts = new List<int> {0, 1, 2, 3, 5, 11, 50, 100, 10_000};

            foreach (var size in sizes)
            foreach (var count in counts)
            {
                yield return new object[] {size, count, Optimize.Minimum};
                yield return new object[] {size, count, Optimize.Maximum};
            }
        }

        [Fact]
        public void SelectNegativeCountArgument()
        {
            Genotype<DoubleGene> F()
            {
                return Genotype.Of(new DoubleChromosome(0.0, 1.0));
            }

            var population = new Population<DoubleGene, double>(2);

            for (int i = 0, n = 2; i < n; ++i)
                population.Add(Phenotype.Of(F(), 12, TestUtils.Ff));

            Assert.Throws<ArgumentOutOfRangeException>(() => Selector().Select(population, -1, Optimize.Maximum));
        }

        [Fact]
        public void SelectNullPopulationArgument()
        {
            Assert.Throws<NullReferenceException>(() => Selector().Select(null, 23, Optimize.Maximum));
        }
    }
}