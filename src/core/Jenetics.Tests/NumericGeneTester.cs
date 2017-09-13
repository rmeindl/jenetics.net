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
    public abstract class NumericGeneTesterBase<TAllele, TGene> : GeneTesterBase<TAllele, TGene>
        where TAllele : IComparable<TAllele>, IConvertible
        where TGene : class, INumericGene<TAllele, TGene>
    {
        [Fact]
        public void CompareTo()
        {
            for (var i = 0; i < 100; ++i)
            {
                var gene1 = Factory()();
                var gene2 = Factory()();

                if (gene1.Allele.CompareTo(gene2.Allele) > 0)
                    Assert.True(gene1.CompareTo(gene2) > 0);
                else if (gene1.Allele.CompareTo(gene2.Allele) < 0)
                    Assert.True(gene1.CompareTo(gene2) < 0);
                else
                    Assert.True(gene1.CompareTo(gene2) == 0);
            }
        }

        [Fact]
        public void MinMax()
        {
            for (var i = 0; i < 100; ++i)
            {
                var gene = Factory()();

                Assert.True(gene.Allele.CompareTo(gene.Min) >= 0);
                Assert.True(gene.Allele.CompareTo(gene.Max) <= 0);
            }
        }

        [Fact]
        public void NewInstanceFromNumber()
        {
            for (var i = 0; i < 100; ++i)
            {
                var gene1 = Factory()();
                var gene2 = gene1.NewInstance(gene1.Allele);

                Assert.Equal(gene2, gene1);
            }
        }
    }
}