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
using Jenetics.Internal.Util;
using Jenetics.Stat;
using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public class TruncationSelectorTest : SelectorTesterBase<TruncationSelector<DoubleGene, double>>
    {
        protected override Factory<TruncationSelector<DoubleGene, double>> Factory()
        {
            return () => new TruncationSelector<DoubleGene, double>();
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
                        new TruncationSelector<DoubleGene, double>(),
                        opt,
                        npopulation,
                        loops
                    );

                    StatisticsAssert.AssertDistribution(distribution, expected.Value, 0.001, 10);
                });
            });
        }

        public static IEnumerable<object[]> ExpectedDistribution()
        {
            const string resource = "Jenetics.resources.TruncationSelector";

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
        public void WorstIndividual()
        {
            const int size = 20;
            var population = new Population<DoubleGene, int>(size);
            for (var i = 0; i < size; ++i)
            {
                var gene = DoubleGene.Of(i, 0, size + 10);
                var ch = DoubleChromosome.Of(gene);
                var gt = Genotype.Of(ch);
                var pt = Phenotype.Of(gt, 1, g => g.Gene.IntValue());

                population.Add(pt);
            }

            var selector = new TruncationSelector<DoubleGene, int>(5);
            var selected = selector.Select(population, 10, Optimize.Minimum);

            foreach (var pt in selected)
                Assert.True(pt.GetFitness() < 5);
        }
    }
}