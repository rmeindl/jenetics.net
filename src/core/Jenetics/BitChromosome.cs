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

namespace Jenetics
{
    [Serializable]
    public class BitChromosome : IChromosome<BitGene>, IComparable<BitChromosome>, IFactory<BitChromosome>
    {
        private readonly byte[] _genes;
        private double _p;
        private BitGeneImmutableSeq _seq;

        private BitChromosome(byte[] bits, int length, double p)
        {
            _genes = bits;
            Length = length;
            _p = p;
            _seq = BitGeneMutableSeq.Of(_genes, length).ToImmutableSeq();
        }

        private BitChromosome(byte[] bits, int length) : this(bits, length == -1 ? bits.Length * 8 : length,
            Bits.Count(bits) / (double) (length == -1 ? bits.Length * 8 : length))
        {
        }

        public BitChromosome(byte[] bits) : this(bits, 0, bits.Length << 3)
        {
        }

        public BitChromosome(byte[] bits, int start, int end) : this(
            Bits.Copy(bits, start, end),
            Math.Min(bits.Length << 3, end) - start,
            0.0
        )
        {
            _p = Bits.Count(_genes) / (double) Length;
        }

        public bool IsValid => true;

        public T As<T>()
        {
            return (T) Convert.ChangeType(this, typeof(T));
        }

        public IEnumerator<BitGene> GetEnumerator()
        {
            return _seq.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IChromosome<BitGene> IFactory<IChromosome<BitGene>>.NewInstance()
        {
            return NewInstance();
        }

        public BitGene GetGene()
        {
            return BitGene.Of(Bits.Get(_genes, 0));
        }

        public BitGene GetGene(int index)
        {
            RangeCheck(index);
            return BitGene.Of(Bits.Get(_genes, index));
        }

        public int Length { get; }

        public IImmutableSeq<BitGene> ToSeq()
        {
            return _seq;
        }

        public IChromosome<BitGene> NewInstance(IImmutableSeq<BitGene> genes)
        {
            if (genes.IsEmpty)
                throw new ArgumentException("The genes sequence must contain at least one gene.");

            var chromosome = new BitChromosome(Bits.NewArray(genes.Length), genes.Length);
            var ones = 0;

            if (genes is BitGeneImmutableSeq seq)
            {
                var iseq = seq;
                iseq.CopyTo(chromosome._genes);
                ones = Bits.Count(chromosome._genes);
            }
            else
            {
                for (var i = genes.Length; --i >= 0;)
                    if (genes[i].BooleanValue())
                    {
                        Bits.Set(chromosome._genes, i);
                        ++ones;
                    }
            }

            // TODO: do not match in copy, arrays do (see ChromosomeTester)
            chromosome._seq = BitGeneMutableSeq.Of(_genes, Length).ToImmutableSeq();

            chromosome._p = ones / (double) genes.Length;
            return chromosome;
        }

        public int CompareTo(BitChromosome other)
        {
            return ToBigInteger().CompareTo(other.ToBigInteger());
        }

        public BitChromosome NewInstance()
        {
            return Of(Length, _p);
        }

        public static BitChromosome Of(string value)
        {
            return new BitChromosome(ToByteArray(value), -1);
        }

        public static BitChromosome Of(BitArray bits)
        {
            return new BitChromosome(bits.ToByteArray(), -1);
        }

        public static BitChromosome Of(BitArray bits, int length)
        {
            var bytes = Bits.NewArray(length);
            for (var i = 0; i < length; ++i)
                if (bits[i])
                    Bits.Set(bytes, i);
            var p = Bits.Count(bytes) / (double) length;

            return new BitChromosome(bytes, length, p);
        }

        public static BitChromosome Of(int length)
        {
            return new BitChromosome(Bits.NewArray(length, 0.5), length, 0.5);
        }

        public static BitChromosome Of(int length, double p)
        {
            return new BitChromosome(Bits.NewArray(length, p), length, p);
        }

        public static BitChromosome Of(BigInteger value)
        {
            return new BitChromosome(value.ToByteArray(), -1);
        }

        public IEnumerable<int> Ones()
        {
            for (var index = 0; index < Length; index++)
            {
                var b = Bits.Get(_genes, index);
                if (b) yield return index;
            }
        }

        public IEnumerable<int> Zeros()
        {
            for (var index = 0; index < Length; index++)
            {
                var b = Bits.Get(_genes, index);
                if (!b) yield return index;
            }
        }

        public bool Get(int index)
        {
            RangeCheck(index);
            return Bits.Get(_genes, index);
        }

        public double GetOneProbability()
        {
            return _p;
        }

        public BigInteger ToBigInteger()
        {
            return new BigInteger(_genes);
        }

        public int BitCount()
        {
            return Bits.Count(_genes);
        }

        public BitChromosome Invert()
        {
            var data = (byte[]) _genes.Clone();
            Bits.Invert(data);
            return new BitChromosome(data, Length, 1.0 - _p);
        }

        public short ShortValue()
        {
            return (short) LongValue();
        }

        public int IntValue()
        {
            return (int) LongValue();
        }

        public long LongValue()
        {
            return (long) ToBigInteger();
        }

        public float FloatValue()
        {
            return LongValue();
        }

        public float DoubleValue()
        {
            return LongValue();
        }

        public BitArray ToBitSet()
        {
            var set = new BitArray(Length);
            for (int i = 0, n = Length; i < n; ++i)
                set[i] = GetGene(i).GetBit();
            return set;
        }

        public int ToByteArray(byte[] bytes)
        {
            if (bytes.Length < _genes.Length)
                throw new IndexOutOfRangeException();

            Array.Copy(_genes, 0, bytes, 0, _genes.Length);
            return _genes.Length;
        }

        public byte[] ToByteArray()
        {
            var data = new byte[_genes.Length];
            ToByteArray(data);
            return data;
        }

        public string ToCanonicalString()
        {
            return string.Join("", ToSeq().Select(g => g.BooleanValue() ? "1" : "0"));
        }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(c =>
            {
                var equals = Length == c.Length;
                for (int i = 0, n = Length; equals && i < n; ++i)
                    equals = GetGene(i) == c.GetGene(i);
                return equals;
            });
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(_genes).Value;
        }

        public override string ToString()
        {
            return Bits.ToByteString(_genes);
        }

        private static byte[] ToByteArray(string value)
        {
            var bytes = Bits.NewArray(value.Length);
            for (var i = value.Length; --i >= 0;)
            {
                var c = value[i];
                if (c == '1')
                    Bits.Set(bytes, i);
                else if (c != '0')
                    throw new ArgumentException($"Illegal character '{c}' at position {i}");
            }

            return bytes;
        }

        private void RangeCheck(int index)
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException(
                    "Index: " + index + ", Length: " + Length
                );
        }
    }
}