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
using System.Text;
using Jenetics.Internal.Math;
using Jenetics.Util;

namespace Jenetics.Internal.Util
{
    public static class Bits
    {
        private const int ByteSize = 8;

        private static readonly byte[] BitSetTable =
        {
            0, 1, 1, 2, 1, 2, 2, 3,
            1, 2, 2, 3, 2, 3, 3, 4,
            1, 2, 2, 3, 2, 3, 3, 4,
            2, 3, 3, 4, 3, 4, 4, 5,
            1, 2, 2, 3, 2, 3, 3, 4,
            2, 3, 3, 4, 3, 4, 4, 5,
            2, 3, 3, 4, 3, 4, 4, 5,
            3, 4, 4, 5, 4, 5, 5, 6,
            1, 2, 2, 3, 2, 3, 3, 4,
            2, 3, 3, 4, 3, 4, 4, 5,
            2, 3, 3, 4, 3, 4, 4, 5,
            3, 4, 4, 5, 4, 5, 5, 6,
            2, 3, 3, 4, 3, 4, 4, 5,
            3, 4, 4, 5, 4, 5, 5, 6,
            3, 4, 4, 5, 4, 5, 5, 6,
            4, 5, 5, 6, 5, 6, 6, 7,
            1, 2, 2, 3, 2, 3, 3, 4,
            2, 3, 3, 4, 3, 4, 4, 5,
            2, 3, 3, 4, 3, 4, 4, 5,
            3, 4, 4, 5, 4, 5, 5, 6,
            2, 3, 3, 4, 3, 4, 4, 5,
            3, 4, 4, 5, 4, 5, 5, 6,
            3, 4, 4, 5, 4, 5, 5, 6,
            4, 5, 5, 6, 5, 6, 6, 7,
            2, 3, 3, 4, 3, 4, 4, 5,
            3, 4, 4, 5, 4, 5, 5, 6,
            3, 4, 4, 5, 4, 5, 5, 6,
            4, 5, 5, 6, 5, 6, 6, 7,
            3, 4, 4, 5, 4, 5, 5, 6,
            4, 5, 5, 6, 5, 6, 6, 7,
            4, 5, 5, 6, 5, 6, 6, 7,
            5, 6, 6, 7, 6, 7, 7, 8
        };

        public static bool GetAndSet(byte[] array, int index)
        {
            var result = Get(array, index);
            Set(array, index);
            return result;
        }

        public static byte[] NewArray(int length)
        {
            return new byte[ToByteLength(length)];
        }

        public static byte[] NewArray(int length, double p)
        {
            var bytes = NewArray(length);

            foreach (var i in random.Indexes(RandomRegistry.GetRandom(), length, p))
                bytes[bit_rol(i, 3)] |= (byte) (1 << (i & 7));

            return bytes;
        }

        public static int ToByteLength(int bitLength)
        {
            return (bitLength & 7) == 0 ? bitLength >> 3 : (bitLength >> 3) + 1;
        }

        public static byte[] Set(byte[] data, int index, bool value)
        {
            return value ? Set(data, index) : Unset(data, index);
        }

        public static byte[] Set(byte[] data, int index)
        {
            data[bit_rol(index, 3)] |= (byte) (1 << (index & 7));
            return data;
        }

        public static byte[] Unset(byte[] data, int index)
        {
            data[bit_rol(index, 3)] &= (byte) ~(1 << (index & 7));
            return data;
        }

        public static bool Get(byte[] data, int index)
        {
            return (data[bit_rol(index, 3)] & (1 << (index & 7))) != 0;
        }

        public static byte[] Copy(byte[] data, int start, int end)
        {
            if (start > end)
                throw new ArgumentException($"start > end: {start} > {end}");
            if (start < 0 || start > data.Length << 3)
                throw new IndexOutOfRangeException($"{start} < 0 || {start} > {data.Length * 8}");

            var to = System.Math.Min(data.Length << 3, end);
            var byteStart = bit_rol(start, 3);
            var bitStart = start & 7;
            var bitLength = to - start;

            var copy = new byte[ToByteLength(to - start)];

            if (copy.Length > 0)
            {
                // Perform the byte wise right shift.
                Array.ConstrainedCopy(data, byteStart, copy, 0, copy.Length);

                // Do the remaining bit wise right shift.
                ShiftRight(copy, bitStart);

                // Add the 'lost' bits from the next byte, if available.
                if (data.Length > copy.Length + byteStart)
                    copy[copy.Length - 1] |= (byte) (data[byteStart + copy.Length]
                                                     << (ByteSize - bitStart));

                // Trim (delete) the overhanging bits.
                copy[copy.Length - 1] &= (byte) bit_rol(0xFF, (copy.Length << 3) - bitLength);
            }

            return copy;
        }

        public static byte[] Invert(byte[] data)
        {
            for (var i = data.Length; --i >= 0;)
                data[i] = (byte) ~data[i];
            return data;
        }

        public static byte[] Complement(byte[] data)
        {
            return Increment(Invert(data));
        }

        public static byte[] Increment(byte[] data)
        {
            var carry = true;
            for (var i = 0; i < data.Length && carry; ++i)
            {
                data[i] = (byte) (data[i] + 1);
                carry = data[i] > 0xFF;
            }

            return data;
        }

        public static byte[] Flip(byte[] data, int index)
        {
            return Get(data, index) ? Unset(data, index) : Set(data, index);
        }

        public static byte[] Reverse(byte[] array)
        {
            var i = 0;
            var j = array.Length;

            while (i < j)
                Swap(array, i++, --j);

            return array;
        }

        private static void Swap(byte[] array, int i, int j)
        {
            var temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        public static void Swap(byte[] data, int start, int end, byte[] otherData, int otherStart)
        {
            for (var i = end - start; --i >= 0;)
            {
                var temp = Get(data, i + start);
                Set(data, i + start, Get(otherData, otherStart + i));
                Set(otherData, otherStart + i, temp);
            }
        }

        public static byte[] ToBytes(long value)
        {
            var bytes = new byte[8];
            bytes[0] = (byte) bit_rol(value, 56);
            bytes[1] = (byte) bit_rol(value, 48);
            bytes[2] = (byte) bit_rol(value, 40);
            bytes[3] = (byte) bit_rol(value, 32);
            bytes[4] = (byte) bit_rol(value, 24);
            bytes[5] = (byte) bit_rol(value, 16);
            bytes[6] = (byte) bit_rol(value, 8);
            bytes[7] = (byte) value;
            return bytes;
        }

        public static long ToLong(byte[] data)
        {
            return
                ((long) data[0] << 56) +
                ((long) (data[1] & 255) << 48) +
                ((long) (data[2] & 255) << 40) +
                ((long) (data[3] & 255) << 32) +
                ((long) (data[4] & 255) << 24) +
                ((data[5] & 255) << 16) +
                ((data[6] & 255) << 8) +
                (data[7] & 255);
        }

        public static string ToByteString(params byte[] data)
        {
            var @out = new StringBuilder();

            if (data.Length > 0)
                for (var j = 7; j >= 0; --j)
                    @out.Append(bit_rol(data[data.Length - 1], j) & 1);
            for (var i = data.Length - 2; i >= 0; --i)
            {
                @out.Append('|');
                for (var j = 7; j >= 0; --j)
                    @out.Append(bit_rol(data[i], j) & 1);
            }

            return @out.ToString();
        }

        public static byte[] FromByteString(string data)
        {
            var parts = data.Split("|");
            var bytes = new byte[parts.Length];

            for (var i = 0; i < parts.Length; ++i)
            {
                if (parts[i].Length != ByteSize)
                    throw new ArgumentException($"Byte value doesn't contain 8 bit: {parts[i]}");

                bytes[parts.Length - 1 - i] = (byte) Convert.ToInt32(parts[i], 2);
            }

            return bytes;
        }

        public static int Count(byte value)
        {
            return BitSetTable[value];
        }

        public static int Count(byte[] data)
        {
            var count = 0;
            for (var i = data.Length; --i >= 0;)
                count += Count(data[i]);
            return count;
        }

        public static byte[] ShiftLeft(byte[] data, int shift)
        {
            var bytes = System.Math.Min(bit_rol(shift, 3), data.Length);
            var bits = shift & 7;

            if (bytes > 0)
            {
                for (int i = 0, n = data.Length - bytes; i < n; ++i)
                    data[data.Length - 1 - i] = data[data.Length - 1 - i - bytes];
                for (var i = 0; i < bytes; ++i)
                    data[i] = 0;
            }
            if (bits > 0 && bytes < data.Length)
            {
                var carry = 0;

                for (var i = bytes; i < data.Length; ++i)
                {
                    var d = data[i] & 0xFF;
                    var nextCarry = bit_rol(d, ByteSize - bits);

                    d <<= bits;
                    d |= carry;
                    data[i] = (byte) (d & 0xFF);

                    carry = nextCarry;
                }
            }

            return data;
        }

        public static byte[] ShiftRight(byte[] data, int shift)
        {
            var bytes = System.Math.Min(bit_rol(shift, 3), data.Length);
            var bits = shift & 7;

            if (bytes > 0)
            {
                for (int i = 0, n = data.Length - bytes; i < n; ++i)
                    data[i] = data[i + bytes];
                for (int i = data.Length, n = data.Length - bytes; --i >= n;)
                    data[i] = 0;
            }
            if (bits > 0 && bytes < data.Length)
            {
                var carry = 0;

                for (var i = data.Length; --i >= 0;)
                {
                    var d = data[i] & 0xFF;
                    var nextCarry = d << (ByteSize - bits);

                    d = bit_rol(d, bits);
                    d |= carry;
                    data[i] = (byte) (d & 0xFF);

                    carry = nextCarry;
                }
            }

            return data;
        }

        public static int bit_rol(int num, int cnt)
        {
            return (int) ((uint) num >> cnt);
        }


        public static long bit_rol(long num, int cnt)
        {
            return (long) ((ulong) num >> cnt);
        }

        public static byte[] ToByteArray(this BitArray bits)
        {
            var ret = new byte[(bits.Length - 1) / ByteSize + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }
    }
}