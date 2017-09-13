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
    public class OptimizeTest
    {
        private static Phenotype<DoubleGene, double> CreatePhenotype(double value)
        {
            return Phenotype.Of(Genotype.Of(DoubleChromosome.Of(DoubleGene.Of(value, 0, 10))), 0, gt => gt.Gene.Allele);
        }

        [Fact]
        public void Best()
        {
            var pt1 = CreatePhenotype(5);
            var pt2 = CreatePhenotype(7);
            var pt3 = CreatePhenotype(7);

            Assert.Same(Optimize.Minimum.Best(pt1, pt2), pt1);
            Assert.Same(Optimize.Maximum.Best(pt1, pt2), pt2);
            Assert.Same(Optimize.Minimum.Best(pt2, pt3), pt2);
        }

        [Fact]
        public void Comparator()
        {
            var comp = Optimize.Maximum.Descending<Phenotype<DoubleGene, double>>();

            Assert.True(comp.Compare(CreatePhenotype(2), CreatePhenotype(3)) > 0);
            Assert.True(comp.Compare(CreatePhenotype(2), CreatePhenotype(2)) == 0);
            Assert.True(comp.Compare(CreatePhenotype(5), CreatePhenotype(3)) < 0);

            comp = Optimize.Minimum.Descending<Phenotype<DoubleGene, double>>();

            Assert.True(comp.Compare(CreatePhenotype(4), CreatePhenotype(3)) > 0);
            Assert.True(comp.Compare(CreatePhenotype(2), CreatePhenotype(2)) == 0);
            Assert.True(comp.Compare(CreatePhenotype(2), CreatePhenotype(3)) < 0);
        }

        [Fact]
        public void Compare()
        {
            var pt1 = CreatePhenotype(5);
            var pt2 = CreatePhenotype(7);
            var pt3 = CreatePhenotype(7);

            Assert.True(Optimize.Minimum.Compare(pt1, pt2) > 0);
            Assert.True(Optimize.Maximum.Compare(pt1, pt2) < 0);
            Assert.True(Optimize.Minimum.Compare(pt3, pt2) == 0);
            Assert.True(Optimize.Maximum.Compare(pt3, pt2) == 0);
        }

        [Fact]
        public void Worst()
        {
            var pt1 = CreatePhenotype(5);
            var pt2 = CreatePhenotype(7);
            var pt3 = CreatePhenotype(7);

            Assert.Same(Optimize.Minimum.Worst(pt1, pt2), pt2);
            Assert.Same(Optimize.Maximum.Worst(pt1, pt2), pt1);
            Assert.Same(Optimize.Minimum.Worst(pt2, pt3), pt2);
        }
    }
}