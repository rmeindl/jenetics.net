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
    public abstract class ChromosomeTesterBase<TGene> : ObjectTesterBase<IChromosome<TGene>>
        where TGene : class, IGene<TGene>
    {
        [Fact]
        public void GetGene()
        {
            var c = Factory()();
            var genes = c.ToSeq();

            Assert.Equal(genes[0], c.GetGene());
            for (var i = 0; i < genes.Length; ++i)
                Assert.Same(genes[i], c.GetGene(i));
        }

        [Fact]
        public void Iterator()
        {
            var c = Factory()();
            var a = c.ToSeq();

            var index = 0;
            foreach (var gene in c)
            {
                Assert.Equal(a[index], gene);
                Assert.Equal(c.GetGene(index), gene);

                ++index;
            }
        }

        [Fact]
        public void Length()
        {
            var c = Factory()();
            var a = c.ToSeq();

            Assert.Equal(a.Length, c.Length);
        }

        [Fact]
        public void NewInstanceFromArray()
        {
            for (var i = 0; i < 100; ++i)
            {
                var c1 = Factory()();
                var genes = c1.ToSeq();
                var c2 = c1.NewInstance(genes);

                Assert.Equal(c1, c2);
                Assert.Equal(c2, c1);
            }
        }

        [Fact]
        public void NewInstanceFromNullArray()
        {
            var c = Factory()();
            Assert.Throws<NullReferenceException>(() => c.NewInstance(null));
        }

        [Fact]
        public void NewInstanceFromRandom()
        {
            for (var i = 0; i < 100; ++i)
            {
                var c1 = Factory()();
                var c2 = c1.NewInstance();

                Assert.Equal(c1.Length, c2.Length);
                if (c1.Equals(c2))
                    Assert.Equal(c1.ToSeq(), c2.ToSeq());
            }
        }
    }
}