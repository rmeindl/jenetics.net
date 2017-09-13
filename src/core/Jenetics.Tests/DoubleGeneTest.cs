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
    public class DoubleGeneTest : NumericGeneTesterBase<double, DoubleGene>
    {
        protected override Factory<DoubleGene> Factory()
        {
            return () => DoubleGene.Of(0, double.MaxValue);
        }

        [Fact]
        public void CreateInvalidNumber()
        {
            var gene = new DoubleGene(0.0, 1.0, 2.0);
            Assert.False(gene.IsValid);
        }

        [Fact]
        public void CreateNumber()
        {
            var gene = new DoubleGene(1.2345, -1234.1234, 1234.1234);
            var g2 = gene.NewInstance(5.0);

            Assert.Equal(5, g2.Allele);
            Assert.Equal(-1234.1234, g2.Min);
            Assert.Equal(1234.1234, g2.Max);
        }

        [Fact]
        public void DoubleGeneIntegerInteger()
        {
            var gene = DoubleGene.Of(-10.567, 10.567);
            Assert.Equal(-10.567, gene.Min);
            Assert.Equal(10.567, gene.Max);
        }

        [Fact]
        public void DoubleGeneIntegerIntegerInteger()
        {
            var gene = new DoubleGene(1.234, 0.345, 2.123);
            Assert.Equal(1.234, gene.Allele);
            Assert.Equal(0.345, gene.Min);
            Assert.Equal(2.123, gene.Max);

            gene = new DoubleGene(0.1, 2.1, 4.1);

            Assert.False(gene.IsValid);
        }

        [Fact]
        public void GetMaxValue()
        {
            var g1 = new DoubleGene(3.2, 0.2, 5.2);
            var g2 = new DoubleGene(4.2, 1.2, 7.2);
            var g3 = new DoubleGene(3.2, 0.2, 5.2);

            Assert.Equal(5.2, g1.Max);
            Assert.Equal(7.2, g2.Max);
            Assert.Equal(5.2, g3.Max);
        }

        [Fact]
        public void GetMinValue()
        {
            var g1 = new DoubleGene(3.1, 0.1, 5.1);
            var g2 = new DoubleGene(4.1, 1.1, 7.1);
            var g3 = new DoubleGene(3.1, 0.1, 5.1);

            Assert.Equal(0.1, g1.Min);
            Assert.Equal(1.1, g2.Min);
            Assert.Equal(0.1, g3.Min);
        }

        [Fact]
        public void Mean()
        {
            const double min = -double.MaxValue;
            const double max = double.MaxValue;
            var template = DoubleGene.Of(min, max);

            for (var i = 1; i < 500; ++i)
            {
                var a = template.NewInstance(i - 50.0);
                var b = template.NewInstance((i - 100) * 3.0);
                var c = a.Mean(b);

                Assert.Equal(min, a.Min);
                Assert.Equal(max, a.Max);
                Assert.Equal(min, b.Min);
                Assert.Equal(max, b.Max);
                Assert.Equal(min, c.Min);
                Assert.Equal(max, c.Max);
                Assert.Equal((i - 50 + (i - 100) * 3) / 2.0, c.Allele);
            }
        }
    }
}