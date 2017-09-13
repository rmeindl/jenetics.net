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

using System.Linq;
using Jenetics.Stat;
using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public class PartiallyMatchedCrossoverTest
    {
        [Theory]
        [ClassData(typeof(TestUtils.AlterProbabilityParameters))]
        public virtual void AlterProbability(int ngenes, int nchromosomes, int npopulation, double p)
        {
            var population = TestUtils.NewPermutationDoubleGenePopulation(
                ngenes, nchromosomes, npopulation
            );

            // The mutator to test.
            var crossover = new PartiallyMatchedCrossover<double, double>(p);

            long nallgenes = ngenes * nchromosomes * npopulation;
            long N = 100;
            var mean = crossover.Order * npopulation * p;

            const long min = 0;
            var max = nallgenes;
            var domain = new Range<long>(min, max);

            var histogram = Histogram.OfLong(min, max, 10);
            var variance = new LongMomentStatistics();

            for (var i = 0; i < N; ++i)
            {
                long alterations = crossover.Alter(population, 1);
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
        public void Crossover()
        {
            var pmco = new PartiallyMatchedCrossover<int, double>(1);

            const int length = 1000;
            var alleles = MutableSeq.OfLength<int>(length).Fill(Factories.Int());
            var ialleles = alleles.ToImmutableSeq();

            var that = alleles.Select(i => new EnumGene<int>(i, ialleles)).ToMutableSeq();
            var other = alleles.Select(i => new EnumGene<int>(i, ialleles)).ToMutableSeq();

            that.Shuffle();
            other.Shuffle();

            var thatChrom1 = new PermutationChromosome<int>(that.ToImmutableSeq());
            Assert.True(thatChrom1.IsValid, "thatChrom1 not valid");

            var otherChrom1 = new PermutationChromosome<int>(other.ToImmutableSeq());
            Assert.True(otherChrom1.IsValid, "otherChrom1 not valid");

            pmco.Crossover(that, other);

            var thatChrom2 = new PermutationChromosome<int>(that.ToImmutableSeq());
            Assert.True(thatChrom2.IsValid, "thatChrom2 not valid: " + thatChrom2.ToImmutableSeq());

            var otherChrom2 = new PermutationChromosome<int>(other.ToImmutableSeq());
            Assert.True(otherChrom2.IsValid, "otherChrom2 not valid: " + otherChrom2.ToImmutableSeq());

            Assert.False(thatChrom1.Equals(thatChrom2), "That chromosome must not be equal");
            Assert.False(otherChrom1.Equals(otherChrom2), "That chromosome must not be equal");
        }

        [Fact]
        public void CrossoverWithIllegalChromosome()
        {
            var pmco = new PartiallyMatchedCrossover<int, double>(1);

            const int length = 1000;
            var alleles = MutableSeq.OfLength<int>(length).Fill(Factories.Int());
            var ialleles = alleles.ToImmutableSeq();

            var that = alleles.Select(i => new EnumGene<int>(i, ialleles)).ToMutableSeq();
            var other = alleles.Select(i => new EnumGene<int>(i, ialleles)).ToMutableSeq();

            pmco.Crossover(that, other);
        }
    }
}