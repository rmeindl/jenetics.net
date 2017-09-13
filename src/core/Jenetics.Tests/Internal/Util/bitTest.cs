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
using Xunit;
using static Jenetics.Util.RandomRegistry;

namespace Jenetics.Internal.Util
{
    public class BitTest
    {
        [Theory]
        [ClassData(typeof(ByteStrDataGenerator))]
        public void ByteStr(byte[] data, string result)
        {
            Assert.Equal(result, Bits.ToByteString(data));
        }

        [Theory]
        [ClassData(typeof(ShiftBitsDataGenerator))]
        public void ShiftLeft(int shift, int bytes)
        {
            var seed = Math.random.Seed();
            var random = new Random(seed);
            var data = new byte[bytes];

            for (var i = 0; i < data.Length * 8; ++i)
                Bits.Set(data, i, random.NextBoolean());

            Bits.ShiftLeft(data, shift);

            random = new Random(seed);
            for (var i = 0; i < shift; ++i)
                Assert.False(Bits.Get(data, i));
            for (int i = shift, n = data.Length * 8; i < n; ++i)
                Assert.Equal(Bits.Get(data, i), random.NextBoolean());
        }

        [Theory]
        [ClassData(typeof(ShiftBitsDataGenerator))]
        public void ShiftRight(int shift, int bytes)
        {
            var seed = Math.random.Seed();
            var random = new Random(seed);
            var data = new byte[bytes];

            for (var i = 0; i < data.Length * 8; ++i)
                Bits.Set(data, i, random.NextBoolean());

            Bits.ShiftRight(data, shift);

            random = new Random(seed);
            for (var i = 0; i < shift; ++i)
            {
                random.NextBoolean();
                Assert.False(Bits.Get(data, data.Length * 8 - 1 - i));
            }
            for (int i = 0, n = data.Length * 8 - shift; i < n; ++i)
                Assert.Equal(Bits.Get(data, i), random.NextBoolean());
        }

        [Theory]
        [ClassData(typeof(IndexoutofboundsDataGenerator))]
        public void SetOutOfIndex(int length, int index)
        {
            var data = Bits.NewArray(length);
            Assert.Throws<IndexOutOfRangeException>(() => Bits.Set(data, index, false));
        }

        [Theory]
        [ClassData(typeof(IndexoutofboundsDataGenerator))]
        public void GetOutOfIndex(int length, int index)
        {
            var data = Bits.NewArray(length);
            Assert.Throws<IndexOutOfRangeException>(() => Bits.Get(data, index));
        }

        private static byte[] NewByteArray(int length, Random random)
        {
            var array = new byte[length];
            for (var i = 0; i < length; ++i)
                array[i] = random.NextByte();
            return array;
        }

        private static int InternalCount(byte value)
        {
            var array = new[] {value};
            var count = 0;
            for (var i = 0; i < 8; ++i)
                if (Bits.Get(array, i))
                    ++count;
            return count;
        }

        private class ByteStrDataGenerator : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] {new[] {(byte) 0}, "00000000"},
                new object[] {new[] {(byte) 1}, "00000001"},
                new object[] {new[] {(byte) 2}, "00000010"},
                new object[] {new[] {(byte) 4}, "00000100"},
                new object[] {new[] {(byte) 0xFF}, "11111111"},

                new object[] {new[] {(byte) 0, (byte) 0}, "00000000|00000000"},
                new object[] {new[] {(byte) 1, (byte) 0}, "00000000|00000001"},
                new object[] {new[] {(byte) 0, (byte) 1}, "00000001|00000000"},
                new object[] {new[] {(byte) 1, (byte) 1}, "00000001|00000001"},

                new object[]
                {
                    Bits.ToBytes(-5165661323090255963L),
                    "10100101|00011111|00111011|00111111|01100101|11100010|01001111|10111000"
                },
                new object[]
                {
                    Bits.ToBytes(-3111444787550306452L),
                    "01101100|10111111|11010010|01101011|01110011|11101101|11010001|11010100"
                },
                new object[]
                {
                    Bits.ToBytes(-3303191740454820247L),
                    "01101001|10100110|00101111|11110101|10011100|10110100|00101000|11010010"
                },
                new object[]
                {
                    Bits.ToBytes(4795980783582945410L),
                    "10000010|10100000|00001111|11001011|00011100|10111111|10001110|01000010"
                },
                new object[]
                {
                    Bits.ToBytes(5363121614382394644L),
                    "00010100|01101101|01111011|01111000|10110001|10100010|01101101|01001010"
                },
                new object[]
                {
                    Bits.ToBytes(-8185663930382162219L),
                    "11010101|11100110|11010111|01011010|00000110|10101110|01100110|10001110"
                },
                new object[]
                {
                    Bits.ToBytes(-1232621285458438758L),
                    "10011010|00010101|10101010|10111001|01111000|11011001|11100100|11101110"
                },
                new object[]
                {
                    Bits.ToBytes(-2081775369634963197L),
                    "00000011|00100001|11000000|10111110|01010100|00001100|00011100|11100011"
                },
                new object[]
                {
                    Bits.ToBytes(-2194074334834473370L),
                    "01100110|11000110|01100011|01100101|00000100|00010101|10001101|11100001"
                },
                new object[]
                {
                    Bits.ToBytes(7950010868533801327L),
                    "01101111|10110001|01110011|10010011|11000011|00011100|01010100|01101110"
                },
                new object[]
                {
                    Bits.ToBytes(6680935979511658057L),
                    "01001001|00110110|11101011|01010001|11100000|01110011|10110111|01011100"
                },
                new object[]
                {
                    Bits.ToBytes(-2670808837407163052L),
                    "01010100|00111101|01011111|01001111|10000011|01100001|11101111|11011010"
                },
                new object[]
                {
                    Bits.ToBytes(4167160717303874479L),
                    "10101111|00000011|11011100|00000100|10011000|10111010|11010100|00111001"
                },
                new object[]
                {
                    Bits.ToBytes(-4513322218647029476L),
                    "00011100|01111001|10101010|11010000|01011010|01110101|01011101|11000001"
                },
                new object[]
                {
                    Bits.ToBytes(564299592671873811L),
                    "00010011|11001011|00011010|01100000|01111101|11001011|11010100|00000111"
                },
                new object[]
                {
                    Bits.ToBytes(5256495800767342066L),
                    "11110010|11100101|00010001|10101000|00010100|11010011|11110010|01001000"
                },
                new object[]
                {
                    Bits.ToBytes(-6440333658299846476L),
                    "10110100|10110000|00001011|11111110|10101000|01010110|10011111|10100110"
                },
                new object[]
                {
                    Bits.ToBytes(8415309172805358741L),
                    "10010101|01010100|00100110|01000000|00011011|00101111|11001001|01110100"
                },
                new object[]
                {
                    Bits.ToBytes(-9216328290938433144L),
                    "10001000|01010001|00111011|11100101|00111111|00000110|00011001|10000000"
                },
                new object[]
                {
                    Bits.ToBytes(2601188737736065391L),
                    "01101111|01000001|10000001|00010010|01100000|01000111|00011001|00100100"
                },
                new object[]
                {
                    Bits.ToBytes(8401653091248721777L),
                    "01110001|01001111|11110001|11111101|11111000|10101010|10011000|01110100"
                },
                new object[]
                {
                    Bits.ToBytes(2560100111339904486L),
                    "11100110|01011101|11011010|10111101|01111100|01001101|10000111|00100011"
                },
                new object[]
                {
                    Bits.ToBytes(928916744534420654L),
                    "10101110|11101100|11100010|10000111|11011011|00101100|11100100|00001100"
                },
                new object[]
                {
                    Bits.ToBytes(-6284404822773081359L),
                    "11110001|11011010|11011011|00100001|00011100|01001111|11001001|10101000"
                },
                new object[]
                {
                    Bits.ToBytes(2811728639172766355L),
                    "10010011|01101010|10101111|11010110|01001100|01000100|00000101|00100111"
                }
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

        private class ShiftBitsDataGenerator : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] {0, 0},
                new object[] {0, 1},
                new object[] {1, 1},
                new object[] {1, 2},
                new object[] {0, 3},
                new object[] {1, 3},
                new object[] {3, 3},
                new object[] {7, 3},
                new object[] {8, 3},
                new object[] {9, 3},
                new object[] {24, 3},
                new object[] {17, 5},
                new object[] {345, 50},
                new object[] {0, 100},
                new object[] {1, 100},
                new object[] {80, 100},
                new object[] {799, 100}
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

        private class IndexoutofboundsDataGenerator : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] {1, 8},
                new object[] {1, -1},
                new object[] {2, 16},
                new object[] {2, 2342},
                new object[] {10, 80},
                new object[] {100, 108}
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
        public void BigShiftLeft()
        {
            var seed = Math.random.Seed();
            var random = new Random(seed);
            var data = new byte[10];

            for (var i = 0; i < data.Length * 8; ++i)
                Bits.Set(data, i, random.NextBoolean());

            Bits.ShiftLeft(data, 100);

            for (var i = 0; i < data.Length * 8; ++i)
                Assert.False(Bits.Get(data, i));
        }

        [Fact]
        public void BigShiftRight()
        {
            var seed = Math.random.Seed();
            var random = new Random(seed);
            var data = new byte[10];

            for (var i = 0; i < data.Length * 8; ++i)
                Bits.Set(data, i, random.NextBoolean());

            Bits.ShiftRight(data, 100);

            for (var i = 0; i < data.Length * 8; ++i)
                Assert.False(Bits.Get(data, i));
        }

        [Fact]
        public void Complement()
        {
            var seed = Math.random.Seed();
            var random = new Random(seed);
            var data = new byte[20];
            random.NextBytes(data);

            var cdata = Bits.Complement((byte[]) data.Clone());
            Assert.NotEqual(data, cdata);
            Assert.Equal(data, Bits.Complement(cdata));
        }

        [Fact]
        public void Count()
        {
            for (int i = byte.MinValue; i <= byte.MaxValue; ++i)
            {
                var value = (byte) i;

                Assert.Equal(Bits.Count(value), InternalCount(value));
            }
        }

        [Fact]
        public void Flip()
        {
            var seed = Math.random.Seed();
            var random = new Random(seed);
            var data = new byte[1000];

            for (var i = 0; i < data.Length; ++i)
                data[i] = random.NextByte();

            var cdata = (byte[]) data.Clone();
            for (var i = 0; i < data.Length * 8; ++i)
                Bits.Flip(cdata, i);

            for (var i = 0; i < data.Length * 8; ++i)
                Assert.Equal(Bits.Get(cdata, i), !Bits.Get(data, i));
        }

        [Fact]
        public void Invert()
        {
            var seed = Math.random.Seed();
            var random = new Random(seed);
            var data = new byte[1000];

            for (var i = 0; i < data.Length * 8; ++i)
                Bits.Set(data, i, random.NextBoolean());

            var cdata = (byte[]) data.Clone();
            Bits.Invert(cdata);

            for (var i = 0; i < data.Length * 8; ++i)
                Assert.Equal(Bits.Get(cdata, i), !Bits.Get(data, i));
        }

        [Fact]
        public void LongToStringFromString()
        {
            var random = GetRandom();
            for (var i = 0; i < 1000; ++i)
            {
                var value = ((long) random.NextInt(32) << 32) + random.NextInt(32);
                var bytes = Bits.ToBytes(value);

                var @string = Bits.ToByteString(bytes);
                var data = Bits.FromByteString(@string);

                Assert.Equal(data, bytes);
                Assert.Equal(Bits.ToLong(data), value);
            }
        }

        [Fact]
        public void Reverse()
        {
            var array = new byte[1000];
            new Random().NextBytes(array);

            var reverseArray = Bits.Reverse((byte[]) array.Clone());
            for (var i = 0; i < array.Length; ++i)
                Assert.Equal(reverseArray[i], array[array.Length - 1 - i]);
        }

        [Fact]
        public void SetGetBit()
        {
            var seed = Math.random.Seed();
            var random = new Random(seed);
            var data = new byte[10000];

            for (var i = 0; i < data.Length * 8; ++i)
                Bits.Set(data, i, random.NextBoolean());

            random = new Random(seed);
            for (var i = 0; i < data.Length * 8; ++i)
                Assert.Equal(Bits.Get(data, i), random.NextBoolean());
        }

        [Fact]
        public void SetGetBit1()
        {
            var data = Enumerable.Repeat((byte) 0, 625).ToArray();

            for (var i = 0; i < data.Length * 8; ++i)
            {
                Bits.Set(data, i);
                Assert.True(Bits.Get(data, i));
            }
        }

        [Fact]
        public void Swap()
        {
            const int byteLength = 1_000;
            const int bitLength = byteLength * 8;

            var seq = NewByteArray(byteLength, new Random());

            for (var start = 0; start < bitLength - 3; ++start)
            {
                var copy = (byte[]) seq.Clone();
                var other = NewByteArray(byteLength, new Random());
                var otherCopy = (byte[]) other.Clone();

                var end = start + 2;
                const int otherStart = 1;

                Bits.Swap(seq, start, end, other, otherStart);

                for (var j = start; j < end; ++j)
                {
                    var actual = Bits.Get(seq, j);
                    var expected = Bits.Get(otherCopy, j + otherStart - start);
                    Assert.Equal(actual, expected);
                }

                for (var j = 0; j < end - start; ++j)
                {
                    var actual = Bits.Get(other, j + otherStart);
                    var expected = Bits.Get(copy, j + start);
                    Assert.Equal(actual, expected);
                }
            }
        }

        [Fact]
        public void ToStringFromString()
        {
            var random = GetRandom();
            for (var i = 0; i < 1000; ++i)
            {
                var bytes = new byte[625];
                random.NextBytes(bytes);

                var @string = Bits.ToByteString(bytes);
                var data = Bits.FromByteString(@string);

                Assert.Equal(data, bytes);
            }
        }
    }
}