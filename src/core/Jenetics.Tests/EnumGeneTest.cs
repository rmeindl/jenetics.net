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
    public class EnumGeneTest : GeneTesterBase<int, EnumGene<int>>
    {
        private readonly Factory<EnumGene<int>> _factory = () =>
        {
            var alleles = MutableSeq.OfLength<int>(100).Fill(Factories.Int()).ToImmutableSeq();
            return EnumGene.Of(alleles);
        };

        protected override Factory<EnumGene<int>> Factory()
        {
            return _factory;
        }

        [Fact]
        public void ValueOf()
        {
            const int length = 100;
            var alleles = MutableSeq.OfLength<int>(length).Fill(Factories.Int()).ToImmutableSeq();

            Assert.Equal(alleles.Length, length);
            for (var i = 0; i < alleles.Length; ++i)
                Assert.Equal(alleles[i], i);

            for (var i = 0; i < alleles.Length; ++i)
            {
                Assert.Equal(new EnumGene<int>(i, alleles).Allele, i);
                Assert.Same(new EnumGene<int>(i, alleles).GetValidAlleles(), alleles);
            }
        }

        [Fact]
        public void ValueOfIndexOutOfBounds1()
        {
            const int length = 100;
            var alleles = MutableSeq.OfLength<int>(length).Fill(Factories.Int()).ToImmutableSeq();

            Assert.Throws<IndexOutOfRangeException>(() => new EnumGene<int>(length + 1, alleles));
        }

        [Fact]
        public void ValueOfIndexOutOfBounds2()
        {
            const int length = 100;
            var alleles = MutableSeq.OfLength<int>(length).Fill(Factories.Int()).ToImmutableSeq();

            Assert.Throws<IndexOutOfRangeException>(() => new EnumGene<int>(-1, alleles));
        }

        [Fact]
        public void ValueOfZeroLength()
        {
            const int length = 0;
            var alleles = MutableSeq.OfLength<int>(length).Fill(Factories.Int()).ToImmutableSeq();

            Assert.Throws<ArgumentException>(() => EnumGene.Of(alleles));
        }
    }
}