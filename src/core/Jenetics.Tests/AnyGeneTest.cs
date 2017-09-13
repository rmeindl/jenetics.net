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
    public class AnyGeneTest : GeneTesterBase<int, AnyGene<int>>
    {
        private class ContinuousRandom : Random
        {
            private int _next;

            public ContinuousRandom(int start)
            {
                _next = start;
            }

            public override int Next()
            {
                return _next++;
            }
        }

        protected override Factory<AnyGene<int>> Factory()
        {
            return () => AnyGene.Of(RandomRegistry.GetRandom().NextInt);
        }

        [Fact]
        public void AllowNullAlleles()
        {
            var gene = AnyGene.Of((int?) null, () => null);

            Assert.Null(gene.Allele);
            for (var i = 0; i < 10; ++i)
                Assert.Null(gene.NewInstance().Allele);
        }

        [Fact]
        public override void IsValid()
        {
            var gene = AnyGene.Of(
                () => RandomRegistry.GetRandom().NextInt(1000),
                i => i < 100
            );

            for (var i = 0; i < 5000; ++i)
            {
                var g = gene.NewInstance();

                Assert.Equal(g.Allele < 100, g.IsValid);
                Assert.True(g.Allele < 1000);
                Assert.True(g.Allele >= 0);
            }
        }

        [Fact]
        public override void NewInstance()
        {
            Random random = new ContinuousRandom(0);
            var gene = AnyGene.Of(random.Next);

            for (var i = 0; i < 100; ++i)
            {
                var g = gene.NewInstance();
                Assert.Equal(i + 1, g.Allele);
            }
        }
    }
}