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
using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public class IntermediateCrossoverTest
    {
        [Fact]
        public void PopulationRecombine()
        {
            RandomRegistry.Using(new Random(123), r =>
            {
                var pop = TestUtils.NewDoubleGenePopulation(5, 1, 2);
                var copy = pop.Copy();

                var recombinator = new IntermediateCrossover<DoubleGene, double>(1);

                recombinator.Alter(pop, 10);

                for (var i = 0; i < pop.Count; ++i)
                {
                    ISeq<DoubleGene> genes = pop[i]
                        .GetGenotype()
                        .GetChromosome()
                        .ToSeq();

                    ISeq<DoubleGene> genesCopy = copy[i]
                        .GetGenotype()
                        .GetChromosome()
                        .ToSeq();

                    foreach (var t in genes)
                        Assert.NotEqual(t, genesCopy[i]);
                }
            });
        }

        [Fact]
        public void Recombine()
        {
            DoubleGene Factory()
            {
                return DoubleGene.Of(0, 100);
            }

            var v = MutableSeq.Of(Factory, 10);
            var w = MutableSeq.Of(Factory, 10);

            var recombinator = new IntermediateCrossover<DoubleGene, double>(0.1, 10);
            recombinator.Crossover(v, w);

            Assert.True(v.ForAll(dg => dg.IsValid));
            Assert.True(w.ForAll(dg => dg.IsValid));
        }}
}