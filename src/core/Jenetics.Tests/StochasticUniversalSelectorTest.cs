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
    public class
        StochasticUniversalSelectorTest : ProbabilitySelectorTesterBase<StochasticUniversalSelector<DoubleGene, double>>
    {
        protected override Factory<StochasticUniversalSelector<DoubleGene, double>> Factory()
        {
            return () => new StochasticUniversalSelector<DoubleGene, double>();
        }

        protected override bool IsSorted()
        {
            return true;
        }

        [Theory]
        [MemberData(nameof(ExpectedDistribution))]
        public void SelectDistribution(Named<double[]> expected, Optimize opt)
        {
            Retry<Exception>(3, () =>
            {
                const int loops = 50;
                const int npopulation = PopulationCount;

                //ThreadLocal<LCG64ShiftRandom> random = new LCG64ShiftRandom.ThreadLocal();
                var random = RandomRegistry.GetRandom();
                RandomRegistry.Using(random, r =>
                {
                    var distribution = Distribution(
                        new StochasticUniversalSelector<DoubleGene, double>(),
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
            const string resource = "Jenetics.resources.StochasticUniversalSelector";

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
        public void SelectMinimum()
        {
            int Ff(Genotype<IntegerGene> gt)
            {
                return gt.GetChromosome().ToSeq().Select(g => g.IntValue()).Sum();
            }

            Genotype<IntegerGene> Gtf()
            {
                return Genotype.Of(IntegerChromosome.Of(0, 100, 10));
            }

            var population = Enumerable.Range(0, 50).Select(i => Phenotype.Of(Gtf(), 50, Ff)).ToPopulation();

            var selector = new StochasticUniversalSelector<IntegerGene, int>();

            var selection = selector.Select(population, 50, Optimize.Minimum);
        }
    }
}