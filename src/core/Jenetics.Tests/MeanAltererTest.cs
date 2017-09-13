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

using Jenetics.Stat;
using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public class MeanAltererTest : AltererTesterBase
    {
        protected override IAlterer<DoubleGene, double> NewAlterer(double p)
        {
            return new MeanAlterer<DoubleGene, double>(p);
        }

        [Theory]
        [ClassData(typeof(TestUtils.AlterProbabilityParameters))]
        public void AlterProbability(int ngenes, int nchromosomes, int npopulation, double p)
        {
            var population = TestUtils.NewDoubleGenePopulation(
                ngenes, nchromosomes, npopulation
            );

            // The mutator to test.
            var mutator = NewAlterer(p);

            long nallgenes = ngenes * nchromosomes * npopulation;
            long N = 100;
            var mean = nallgenes * p;

            const long min = 0;
            var max = nallgenes;
            var domain = new Range<long>(min, max);

            var histogram = Histogram.OfLong(min, max, 10);
            var variance = new LongMomentStatistics();

            for (var i = 0; i < N; ++i)
            {
                long alterations = mutator.Alter(population, 1);
                histogram.Accept(alterations);
                variance.Accept(alterations);
            }

            // Normal distribution as approximation for binomial distribution.
            // TODO: Implement test
//		assertDistribution(
//			histogram,
//			new NormalDistribution<>(domain, mean, variance.getVariance())
//		);
        }

        [Fact]
        public void Recombinate()
        {
            const int ngenes = 11;
            const int nchromosomes = 9;
            const int npopulation = 100;
            var p1 = TestUtils.NewDoubleGenePopulation(ngenes, nchromosomes, npopulation);
            var p2 = p1.Copy();
            var selected = new[] {3, 34};

            var crossover = new MeanAlterer<DoubleGene, double>(0.1);
            crossover.Recombine(p1, selected, 3);

            Assert.Equal(TestUtils.Diff(p1, p2), ngenes);
        }
    }
}