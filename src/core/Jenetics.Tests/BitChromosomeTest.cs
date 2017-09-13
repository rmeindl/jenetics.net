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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Jenetics.Internal.Util;
using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public class BitChromosomeTest : ChromosomeTesterBase<BitGene>
    {
        [Theory]
        [ClassData(typeof(BitCountProbabilityDataGenerator))]
        public void BitCount(double p)
        {
            const int size = 1_000;
            var @base = BitChromosome.Of(size, p);

            for (var i = 0; i < 1_000; ++i)
            {
                var other = @base.NewInstance();

                var bitCount = other.Count(gene => gene.BooleanValue());

                Assert.Equal(bitCount, other.BitCount());
            }
        }

        [Theory]
        [ClassData(typeof(BitCountProbabilityDataGenerator))]
        public void BitSetBitCount(double p)
        {
            const int size = 1_000;
            var @base = BitChromosome.Of(size, p);

            for (var i = 0; i < 1_000; ++i)
            {
                var other = @base.NewInstance();
                Assert.Equal(other.BitCount(), other.ToBitSet().OfType<bool>().Count(bit => bit));
            }
        }

        protected override Factory<IChromosome<BitGene>> Factory()
        {
            return () => BitChromosome.Of(500, 0.3);
        }

        private class BitCountProbabilityDataGenerator : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] {0.01},
                new object[] {0.1},
                new object[] {0.125},
                new object[] {0.333},
                new object[] {0.5},
                new object[] {0.75},
                new object[] {0.85},
                new object[] {0.999}
            };

            public IEnumerator<object[]> GetEnumerator()
            {
                return _data.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Fact]
        public void BitChromosomeBitSet()
        {
            var bits = new BitArray(10);
            for (var i = 0; i < 10; ++i)
                bits[i] = i % 2 == 0;

            var c = BitChromosome.Of(bits);
            for (var i = 0; i < bits.Length; ++i)
                Assert.Equal(c.GetGene(i).GetBit(), i % 2 == 0);
        }

        [Fact]
        public void ChromosomeProbability()
        {
            var data = new byte[1234];
            RandomRegistry.GetRandom().NextBytes(data);

            var c = new BitChromosome(data);
            Assert.Equal(Bits.Count(data) / (double) (data.Length * 8), c.GetOneProbability());
        }

        [Fact]
        public void FromBitSet()
        {
            var random = RandomRegistry.GetRandom();
            var bits = new BitArray(2343);
            for (var i = 0; i < bits.Count; ++i)
                bits[i] = random.NextBoolean();

            var c = BitChromosome.Of(bits);
            Assert.Equal(bits.ToByteArray(), c.ToByteArray());
        }

        [Fact]
        public void FromByteArrayBitSet()
        {
            var random = RandomRegistry.GetRandom();
            var bytes = new byte[234];
            random.NextBytes(bytes);

            var bits = new BitArray(bytes);
            var c = BitChromosome.Of(bits);
            Assert.Equal(bytes, c.ToByteArray());
            Assert.Equal(bytes, bits.ToByteArray());
        }

        [Fact]
        public void IntProbability()
        {
            var c = BitChromosome.Of(10, 0);
            foreach (var g in c)
                Assert.False(g.GetBit());

            c = BitChromosome.Of(10, 1);
            foreach (var g in c)
                Assert.True(g.GetBit());
        }

        [Fact]
        public void Invert()
        {
            var c1 = BitChromosome.Of(100, 0.3);
            var c3 = c1.Invert();

            for (var i = 0; i < c1.Length; ++i)
                Assert.True(c1.GetGene(i).GetBit() != c3.GetGene(i).GetBit());

            var c4 = c3.Invert();
            Assert.Equal(c1, c4);
        }

        [Fact]
        public void NewInstance()
        {
            const int size = 50_000;
            var @base = BitChromosome.Of(size, 0.5);

            for (var i = 0; i < 100; ++i)
            {
                var other = @base.NewInstance();
                Assert.NotEqual(@base, other);

                Assert.True(Math.Abs(1 - other.BitCount() / (size / 2.0)) < 0.02);
            }
        }

        [Fact]
        public void NumValue()
        {
            var c1 = BitChromosome.Of(10);

            var value = c1.IntValue();
            Assert.Equal((short) value, c1.ShortValue());
            Assert.Equal(value, c1.LongValue());
            Assert.Equal(value, c1.FloatValue());
            Assert.Equal((double) value, c1.DoubleValue());
        }

        [Fact]
        public void Ones()
        {
            var c = BitChromosome.Of(1000, 0.5);

            var ones = c.Ones().Count();
            Assert.Equal(ones, c.BitCount());
            Assert.True(c.Ones().All(i => c.Get(i)));
        }

        [Fact]
        public void SeqTypes()
        {
            var c = BitChromosome.Of(100, 0.3);

            Assert.Equal(typeof(BitGeneImmutableSeq), c.ToSeq().GetType());
            Assert.Equal(typeof(BitGeneMutableSeq), c.ToSeq().Copy().GetType());
            Assert.Equal(typeof(BitGeneImmutableSeq), c.ToSeq().Copy().ToImmutableSeq().GetType());
        }

        [Fact]
        public void ToBigInteger()
        {
            var data = new byte[1056];
            RandomRegistry.GetRandom().NextBytes(data);
            var value = new BigInteger(data);
            var chromosome = BitChromosome.Of(value);

            Assert.Equal(chromosome.ToBigInteger(), value);
        }

        [Fact]
        public void ToBitSet()
        {
            var c1 = BitChromosome.Of(34);
            var c2 = BitChromosome.Of(c1.ToBitSet(), 34);

            for (var i = 0; i < c1.Length; ++i)
                Assert.Equal(c1.GetGene(i).GetBit(), c2.GetGene(i).GetBit());
        }

        [Fact]
        public void ToByteArray()
        {
            var random = RandomRegistry.GetRandom();
            var data = new byte[16];
            for (var i = 0; i < data.Length; ++i)
                data[i] = (byte) (random.Next() * 256);
            var bc = new BitChromosome(data);

            Assert.Equal(data, bc.ToByteArray());
        }

        [Fact]
        public void ToCanonicalString()
        {
            var c = BitChromosome.Of(new BigInteger(234902));
            var value = c.ToCanonicalString();
            var sc = BitChromosome.Of(value);

            Assert.Equal(c, sc);
        }

        [Fact]
        public void ToStringToByteArray()
        {
            var random = RandomRegistry.GetRandom();
            var data = new byte[10];
            for (var i = 0; i < data.Length; ++i)
                data[i] = (byte) (random.Next() * 256);

            var dataString = Bits.ToByteString(data);

            var sdata = Bits.FromByteString(dataString);
            Assert.Equal(data, sdata);
        }

        [Fact]
        public void Zeros()
        {
            var c = BitChromosome.Of(1000, 0.5);

            var zeros = c.Zeros().Count();
            Assert.Equal(zeros, c.Length - c.BitCount());
            Assert.True(c.Zeros().All(i => !c.Get(i)));
        }
    }
}