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
using Jenetics.Stat;
using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public class UniformCrossoverTest : AltererTesterBase
    {
        protected override IAlterer<DoubleGene, double> NewAlterer(double p)
        {
            return new UniformCrossover<DoubleGene, double>(p);
        }

        [Fact]
        public void Crossover()
        {
            var g1 = CharacterChromosome.Of("1234567890").ToSeq();
            var g2 = CharacterChromosome.Of("abcdefghij").ToSeq();

            RandomRegistry.Using(new Random(10), r =>
            {
                var crossover = new UniformCrossover<CharacterGene, char>(0.5, 0.5);

                var g1C = g1.Copy();
                var g2C = g2.Copy();
                var changed = crossover.Crossover(g1C, g2C);

                Assert.Equal(Enumerable.Range(0, g2C.Length).Count(i => char.IsDigit(g2C[i].Allele)), changed);
            });
        }

        [Fact]
        public void CrossoverChanges()
        {
            var g1 = CharacterChromosome.Of("1234567890").ToSeq();
            var g2 = CharacterChromosome.Of("abcdefghij").ToSeq();

            RandomRegistry.Using(new Random(10), r =>
            {
                var crossover = new UniformCrossover<CharacterGene, char>(0.5, 0.5);

                var statistics = new DoubleMomentStatistics();

                for (var j = 0; j < 1000; ++j)
                {
                    var g1C = g1.Copy();
                    var g2C = g2.Copy();
                    var changed = crossover.Crossover(g1C, g2C);

                    Assert.Equal(Enumerable
                            .Range(0, g2C.Length)
                            .Count(i => char.IsDigit(g2C[i].Allele)), changed
                    );

                    statistics.Accept(changed);
                }

                Assert.Equal(5.0, statistics.Mean, 1);
            });
        }
    }
}