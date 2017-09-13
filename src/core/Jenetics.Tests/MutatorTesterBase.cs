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
using static Jenetics.TestUtils;

namespace Jenetics
{
    public abstract class MutatorTesterBase : AltererTesterBase
    {
        [Theory]
        [ClassData(typeof(AlterCountParameters))]
        public virtual void AlterCount(
            int ngenes,
            int nchromosomes,
            int npopulation
        )
        {
            var p1 = NewDoubleGenePopulation(
                ngenes, nchromosomes, npopulation
            );
            var p2 = p1.Copy();
            Assert.Equal(p2, p1);

            var mutator = NewAlterer(0.01);

            var mutations = mutator.Alter(p1, 1);
            var difference = Diff(p1, p2);

            Assert.Equal(mutations, difference);
        }

        [Theory]
        [ClassData(typeof(AlterProbabilityParameters))]
        public virtual void AlterProbability(int ngenes, int nchromosomes, int npopulation, double p)
        {
            var population = NewDoubleGenePopulation(
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
    }
}