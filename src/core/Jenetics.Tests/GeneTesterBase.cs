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

using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public abstract class GeneTesterBase<TAllele, TGene> : ObjectTesterBase<TGene>
        where TGene : class, IGene<TAllele, TGene>
    {
        [Fact]
        public void AlleleNotNull()
        {
            for (var i = 0; i < 1000; ++i)
                Assert.NotNull(Factory()().Allele);
        }

        [Fact]
        public void EqualsAllele()
        {
            var same = NewEqualObjects(5);

            var that = same[0];
            for (var i = 1; i < same.Length; ++i)
            {
                var other = same[i];

                Assert.Equal(other.Allele, other.Allele);
                Assert.Equal(other.Allele, that.Allele);
                Assert.Equal(that.Allele, other.Allele);
            }
        }

        [Fact]
        public virtual void NewInstance()
        {
            for (var i = 0; i < 1000; ++i)
            {
                var gene = Factory()();
                Assert.NotNull(gene);
                Assert.NotNull(gene.Allele);
                Assert.True(gene.IsValid);

                var gene2 = gene.NewInstance();
                Assert.NotNull(gene2);
                Assert.NotNull(gene2.Allele);
                Assert.True(gene2.IsValid);
            }
        }

        [Fact]
        public void NotEqualsAllele()
        {
            for (var i = 0; i < 1000; ++i)
            {
                var that = Factory()().Allele;
                var other = Factory()().Allele;

                if (that.Equals(other))
                {
                    Assert.True(other.Equals(that));
                    Assert.Equal(that.GetHashCode(), other.GetHashCode());
                }
                else
                {
                    Assert.False(other.Equals(that));
                    Assert.False(that.Equals(other));
                }
            }
        }

        [Fact]
        public void NotEqualsAlleleNull()
        {
            var that = Factory()().Allele;
            Assert.False(that.Equals(null));
        }
    }
}