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

using System.Collections.Generic;
using System.Linq;
using Jenetics.Internal.Math;
using Jenetics.Util;
using Xunit;
using Xunit.Sdk;

namespace Jenetics
{
    public class PermutationChromosomeTest : ChromosomeTesterBase<EnumGene<int>>
    {
        protected override Factory<IChromosome<EnumGene<int>>> Factory()
        {
            return () => PermutationChromosome.OfInteger(100);
        }

        private static void AssertUnique<T>(ISeq<T> seq)
        {
            ISet<T> set = new HashSet<T>(seq.ToList());
            if (seq.Length > set.Count)
                throw new XunitException("Sequence elements are not unique: " + seq);
        }

        [Fact]
        public void InvalidChromosome()
        {
            var alleles = ImmutableSeq.Of(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            var gene = new EnumGene<int>(3, alleles);
            var genes = MutableSeq.OfLength<EnumGene<int>>(10).Fill(() => gene).ToImmutableSeq();

            var chromosome = new PermutationChromosome<int>(genes);
            Assert.False(chromosome.IsValid);
        }

        [Fact]
        public void IsNotValid()
        {
            var alleles = Enumerable.Range(0, 100).ToImmutableSeq();

            var genes = Base.Subset(100, 10).Select(i => EnumGene.Of(i % 3, alleles)).ToImmutableSeq();

            var ch = new PermutationChromosome<int>(genes);

            Assert.False(ch.IsValid);
            Assert.Equal(10, ch.Length);
        }

        [Fact]
        public override void IsValid()
        {
            var alleles = Enumerable.Range(0, 100).ToImmutableSeq();

            var genes = Base.Subset(100, 10).Select(i => EnumGene.Of(i, alleles)).ToImmutableSeq();

            var ch = new PermutationChromosome<int>(genes);
            Assert.True(ch.IsValid);
            Assert.Equal(10, ch.Length);
        }

        [Fact]
        public void OfIntegerLength()
        {
            var c = PermutationChromosome.OfInteger(100);
            var genes = c.GetValidAlleles().Copy();
            var genesOrdered = genes.OrderBy(g => g).ToList();

            for (var i = 0; i < c.Length; ++i)
                Assert.Equal(i, genesOrdered[i]);
        }

        [Fact]
        public void OfIntegerRangeLength()
        {
            var c1 = PermutationChromosome.OfInteger(IntRange.Of(0, 2000), 1000);
            Assert.True(c1.IsValid);

            var c2 = PermutationChromosome.OfInteger(IntRange.Of(0, 2000), 1000);
            Assert.True(c2.IsValid);

            var m1 = c1.ToSeq().Copy();
            var m2 = c2.ToSeq().Copy();
            AssertUnique(m1);
            AssertUnique(m2);

            var pmx = new PartiallyMatchedCrossover<int, double>(1);

            pmx.Crossover(m1, m2);
            AssertUnique(m1);
            AssertUnique(m2);
        }

        [Fact]
        public void OfIntegerStartEnd()
        {
            var c = PermutationChromosome.OfInteger(100, 200);
            var genes = c.GetValidAlleles().Copy();
            var genesOrdered = genes.OrderBy(g => g).ToList();

            for (var i = 0; i < c.Length; ++i)
                Assert.Equal(i + 100, genesOrdered[i]);
        }
    }
}