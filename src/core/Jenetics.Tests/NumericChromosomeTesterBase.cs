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
using Xunit;

namespace Jenetics
{
    public abstract class NumericChromosomeTesterBase<TAllele, TGene> : ChromosomeTesterBase<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
        where TGene : class, INumericGene<TAllele, TGene>
    {
        /*
        [Fact]
        public void PrimitiveTypeAccess() {
            var c = (INumericChromosome<N, G>) Factory()();

            Assert.Equal(c.ByteValue(), c.ByteValue(0));
            Assert.Equal(c.ShortValue(), c.ShortValue(0));
            Assert.Equal(c.IntValue(), c.IntValue(0));
            Assert.Equal(c.FloatValue(), c.FloatValue(0));
            Assert.Equal(c.DoubleValue(), c.DoubleValue(0));
        }
        */

        private static void AssertMinMax(INumericChromosome<TAllele, TGene> c1, INumericChromosome<TAllele, TGene> c2)
        {
            Assert.Equal(c1.Min, c2.Min);
            Assert.Equal(c1.Max, c2.Max);
        }

        private static void AssertValid(INumericChromosome<TAllele, TGene> c)
        {
            if (c.IsValid)
                foreach (var gene in c)
                {
                    Assert.True(gene.Allele.CompareTo(c.Min) >= 0);
                    Assert.True(gene.Allele.CompareTo(c.Max) <= 0);
                }
            else
                foreach (var gene in c)
                    Assert.True(
                        gene.Allele.CompareTo(c.Min) < 0 ||
                        gene.Allele.CompareTo(c.Max) > 0
                    );
        }

        [Fact]
        public void GeneMinMax()
        {
            var c = (INumericChromosome<TAllele, TGene>) Factory()();

            foreach (var gene in c)
            {
                Assert.Equal(c.Min, gene.Min);
                Assert.Equal(c.Max, gene.Max);
            }
        }

        [Fact]
        public void MinMax()
        {
            var c1 = (INumericChromosome<TAllele, TGene>) Factory()();

            var c2 = (INumericChromosome<TAllele, TGene>) Factory()();


            AssertMinMax(c1, c2);
            AssertValid(c1);
            AssertValid(c2);
        }
    }
}