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
using Jenetics.Internal.Util;
using Jenetics.Stat;
using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public class RouletteWheelSelectorTest : ProbabilitySelectorTesterBase<RouletteWheelSelector<DoubleGene, double>>
    {
        protected override Factory<RouletteWheelSelector<DoubleGene, double>> Factory()
        {
            return () => new RouletteWheelSelector<DoubleGene, double>();
        }

        protected override bool IsSorted()
        {
            return false;
        }

        [Theory]
        [MemberData(nameof(ExpectedDistribution))]
        public void SelectDistribution(Named<double[]> expected, Optimize opt)
        {
            Retry<Exception>(3, () =>
            {
                const int loops = 50;
                var npopulation = PopulationCount;

                //ThreadLocal<LCG64ShiftRandom> random = new LCG64ShiftRandom.ThreadLocal();
                var random = RandomRegistry.GetRandom();
                RandomRegistry.Using(random, r =>
                {
                    var distribution = Distribution(
                        new RouletteWheelSelector<DoubleGene, double>(),
                        opt,
                        npopulation,
                        loops
                    );

                    StatisticsAssert.AssertDistribution(distribution, expected.Value, 0.001, 5);
                });
            });
        }

        public static IEnumerable<object[]> ExpectedDistribution()
        {
            const string resource = "Jenetics.resources.RouletteWheelSelector";

            return new[] {Optimize.Maximum, Optimize.Minimum}
                .Select(opt =>
                {
                    var data = TestData.Of(resource, opt.ToString().ToUpper());
                    var expected = data
                        .Select(line => double.Parse(line[0]))
                        .ToArray();

                    return new object[] {Named<double[]>.Of("distribution", expected), opt};
                }).ToArray();
        }

        [Fact]
        public void Maximize()
        {
            RandomRegistry.Using(new Random(), r =>
            {
                Func<Genotype<IntegerGene>, int> ff =
                    g => g.GetChromosome().GetGene().Allele;

                Phenotype<IntegerGene, int> F()
                {
                    return Phenotype.Of(Genotype.Of(IntegerChromosome.Of(0, 100)), 1, ff);
                }

                var population = Enumerable.Range(0, 1000)
                    .Select(i => F())
                    .ToPopulation();

                var selector =
                    new RouletteWheelSelector<IntegerGene, int>();

                var p = selector.Probabilities(population, 100, Optimize.Maximum);
                Assert.True(ProbabilitySelector.Sum2One(p), p + " != 1");
            });
        }

        [Fact]
        public void Minimize()
        {
            RandomRegistry.Using(new Random(), r =>
            {
                int Ff(Genotype<IntegerGene> g)
                {
                    return g.GetChromosome().GetGene().Allele;
                }

                Phenotype<IntegerGene, int> F()
                {
                    return Phenotype.Of(Genotype.Of(IntegerChromosome.Of(0, 100)), 1, Ff);
                }

                var population = Enumerable.Range(0, 1000)
                    .Select(i => F())
                    .ToPopulation();

                var selector = new RouletteWheelSelector<IntegerGene, int>();

                var p = selector.Probabilities(population, 100, Optimize.Minimum);
                Assert.True(ProbabilitySelector.Sum2One(p), p + " != 1");
            });
        }
    }
}