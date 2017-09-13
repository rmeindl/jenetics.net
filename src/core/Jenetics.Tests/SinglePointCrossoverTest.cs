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
    public class SinglePointCrossoverTest : AltererTesterBase
    {
        private class ConstRandom : Random
        {
            private readonly int _value;

            public ConstRandom(int value)
            {
                _value = value;
            }

            public override int Next()
            {
                return _value;
            }

            public override int Next(int maxValue)
            {
                return _value;
            }
        }

        protected override IAlterer<DoubleGene, double> NewAlterer(double p)
        {
            return new SinglePointCrossover<DoubleGene, double>(p);
        }

        [Fact]
        public void Crossover()
        {
            var chars = CharSeq.Of("a-zA-Z");

            var g1 = new CharacterChromosome(chars, 20).ToSeq();
            var g2 = new CharacterChromosome(chars, 20).ToSeq();

            const int rv1 = 12;
            RandomRegistry.Using(new ConstRandom(rv1), r =>
            {
                var crossover = new SinglePointCrossover<CharacterGene, double>();

                var g1C = MutableSeq.Of<CharacterGene>(g1);
                var g2C = MutableSeq.Of<CharacterGene>(g2);
                crossover.Crossover(g1C, g2C);

                Assert.Equal(g1C.ToImmutableSeq().SubSeq(0, rv1), g1.SubSeq(0, rv1));
                Assert.Equal(g1C.SubSeq(rv1), g2.SubSeq(rv1));
                Assert.NotEqual(g1C, g2);
                Assert.NotEqual(g2C, g1);

                const int rv2 = 0;
                RandomRegistry.Using(new ConstRandom(rv2), r2 =>
                {
                    var g1C2 = MutableSeq.Of<CharacterGene>(g1);
                    var g2C2 = MutableSeq.Of<CharacterGene>(g2);
                    crossover.Crossover(g1C2, g2C2);
                    Assert.Equal(g1C2, g2);
                    Assert.Equal(g2C2, g1);
                    Assert.Equal(g1C2.SubSeq(0, rv2), g1.SubSeq(0, rv2));
                    Assert.Equal(g1C2.SubSeq(rv2), g2.SubSeq(rv2));

                    const int rv3 = 1;
                    RandomRegistry.Using(new ConstRandom(rv3), r3 =>
                    {
                        var g1C3 = MutableSeq.Of<CharacterGene>(g1);
                        var g2C3 = MutableSeq.Of<CharacterGene>(g2);
                        crossover.Crossover(g1C3, g2C3);
                        Assert.Equal(g1C3.SubSeq(0, rv3), g1.SubSeq(0, rv3));
                        Assert.Equal(g1C3.SubSeq(rv3), g2.SubSeq(rv3));

                        var rv4 = g1.Length;
                        RandomRegistry.Using(new ConstRandom(rv4), r4 =>
                        {
                            var g1C4 = MutableSeq.Of<CharacterGene>(g1);
                            var g2C4 = MutableSeq.Of<CharacterGene>(g2);
                            crossover.Crossover(g1C4, g2C);
                            Assert.Equal(g1C4, g1);
                            Assert.Equal(g2C4, g2);
                            Assert.Equal(g1C4.SubSeq(0, rv4), g1.SubSeq(0, rv4));
                            Assert.Equal(g1C4.SubSeq(rv4), g2.SubSeq(rv4));
                        });
                    });
                });
            });
        }
    }
}