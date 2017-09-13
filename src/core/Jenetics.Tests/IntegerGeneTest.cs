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
    public class IntegerGeneTest : NumericGeneTesterBase<int, IntegerGene>
    {
        private readonly Factory<IntegerGene> _factory = () => IntegerGene.Of(0, int.MaxValue);

        protected override Factory<IntegerGene> Factory()
        {
            return _factory;
        }

        [Fact]
        public void CreateInvalidNumber()
        {
            var gene = IntegerGene.Of(0, 1, 2);

            Assert.False(gene.IsValid);
        }

        [Fact]
        public void CreateNumber()
        {
            var gene = IntegerGene.Of(1, 0, 12);
            var g2 = gene.NewInstance(5);

            Assert.Equal(5, g2.Allele);
            Assert.Equal(0, g2.Min);
            Assert.Equal(12, g2.Max);
        }

        [Fact]
        public void Mean()
        {
            const int min = -int.MaxValue;
            const int max = int.MaxValue;
            var template = IntegerGene.Of(min, max);

            for (var i = 1; i < 500; ++i)
            {
                var a = template.NewInstance(i - 50);
                var b = template.NewInstance((i - 100) * 3);
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
        public void Parameters()
        {
            var gene = IntegerGene.Of(10, 10);

            Assert.Equal(10, gene.Min);
            Assert.Equal(10, gene.Max);
            Assert.Equal(10, gene.Allele);
        }

        [Fact]
        public void Set()
        {
            var gene = new IntegerGene(5, 0, 10);
            Assert.Equal(5, gene.Allele);
            Assert.Equal(0, gene.Min);
            Assert.Equal(10, gene.Max);
        }
    }
}