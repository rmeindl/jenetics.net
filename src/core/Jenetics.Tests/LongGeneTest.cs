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

using Xunit;

namespace Jenetics
{
    public class LongGeneTest : NumericGeneTesterBase<long, LongGene>
    {
        private readonly Factory<LongGene> _factory = () => LongGene.Of(0, long.MaxValue);

        protected override Factory<LongGene> Factory()
        {
            return _factory;
        }

        [Fact]
        public void CreateInvalidNumber()
        {
            var gene = LongGene.Of(0, 1, 2);
            Assert.False(gene.IsValid);
        }

        [Fact]
        public void CreateNumber()
        {
            var gene = LongGene.Of(1, 0, 12);
            var g2 = gene.NewInstance(5L);

            Assert.Equal(5, g2.Allele);
            Assert.Equal(0, g2.Min);
            Assert.Equal(12, g2.Max);
        }

        [Fact]
        public void Mean()
        {
            const long min = -int.MaxValue;
            const long max = int.MaxValue;
            var template = LongGene.Of(min, max);

            for (var i = 1; i < 500; ++i)
            {
                var a = template.NewInstance(i - 50L);
                var b = template.NewInstance((i - 100L) * 3);
                var c = a.Mean(b);

                Assert.Equal(min, a.Min);
                Assert.Equal(max, a.Max);
                Assert.Equal(min, b.Min);
                Assert.Equal(max, b.Max);
                Assert.Equal(min, c.Min);
                Assert.Equal(max, c.Max);
                Assert.Equal((i - 50 + (i - 100) * 3) / 2, c.Allele);
            }
        }

        [Fact]
        public void Set()
        {
            var gene = new LongGene(5L, 0L, 10L);
            Assert.Equal(5, gene.Allele);
            Assert.Equal(0, gene.Min);
            Assert.Equal(10, gene.Max);
        }
    }
}