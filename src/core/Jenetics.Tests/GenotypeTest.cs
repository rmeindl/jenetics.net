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
    public class GenotypeTest : ObjectTesterBase<Genotype<DoubleGene>>
    {
        private readonly Factory<Genotype<DoubleGene>> _factory = () => Genotype.Of(
            DoubleChromosome.Of(0, 1, 50),
            DoubleChromosome.Of(0, 1, 500),
            DoubleChromosome.Of(0, 1, 100),
            DoubleChromosome.Of(0, 1, 50)
        );

        protected override Factory<Genotype<DoubleGene>> Factory()
        {
            return _factory;
        }

        [Fact]
        public void FactoryTest()
        {
            var factory = _factory;
            var gt = _factory();

            Assert.Equal(gt.Length, factory().Length);
            Assert.Equal(gt.GetNumberOfGenes(), factory().GetNumberOfGenes());
            for (var i = 0; i < factory().Length; ++i)
                Assert.Equal(
                    gt.GetChromosome(i).Length,
                    factory().GetChromosome(i).Length
                );
        }

        [Fact]
        public void NewInstance()
        {
            var gt1 = Genotype.Of(
                //Rotation
                DoubleChromosome.Of(DoubleGene.Of(-Math.PI, Math.PI)),

                //Translation
                DoubleChromosome.Of(DoubleGene.Of(-300, 300), DoubleGene.Of(-300, 300)),

                //Shear
                DoubleChromosome.Of(DoubleGene.Of(-0.5, 0.5), DoubleGene.Of(-0.5, 0.5))
            );

            var gt2 = gt1.NewInstance();

            Assert.Equal(gt1.Length, gt2.Length);
            for (var i = 0; i < gt1.Length; ++i)
            {
                var ch1 = gt1.GetChromosome(i);
                var ch2 = gt2.GetChromosome(i);
                Assert.Equal(ch1.Length, ch2.Length);
            }
        }

        [Fact]
        public void NumberOfGenes()
        {
            var genotype = Genotype.Of(
                DoubleChromosome.Of(0.0, 1.0, 8),
                DoubleChromosome.Of(1.0, 2.0, 10),
                DoubleChromosome.Of(0.0, 10.0, 9),
                DoubleChromosome.Of(0.1, 0.9, 5)
            );
            Assert.Equal(32, genotype.GetNumberOfGenes());
        }

        [Fact]
        public void TestCreate()
        {
            var c1 = LongChromosome.Of(0, 100, 10);
            var c2 = LongChromosome.Of(0, 100, 10);
            var g1 = Genotype.Of(c1, c2);
            var g2 = g1.NewInstance();

            Assert.False(ReferenceEquals(g1, g2));
            Assert.False(g1.Equals(g2));
        }

        [Fact]
        public void TestGenotypeGenotypeOfT()
        {
            var c1 = BitChromosome.Of(12);
            var c2 = BitChromosome.Of(12);
            var g2 = Genotype.Of(c1, c2, c2);
            var g4 = g2;

            Assert.Equal(g4, g2);
            Assert.Equal(g4.GetHashCode(), g2.GetHashCode());
        }

        [Fact]
        public void TestSetGetChromosome()
        {
            var c1 = LongChromosome.Of(0, 100, 10);
            var c2 = LongChromosome.Of(0, 100, 10);
            var c3 = LongChromosome.Of(0, 100, 10);
            var g = Genotype.Of(c1, c2);
        }
    }
}