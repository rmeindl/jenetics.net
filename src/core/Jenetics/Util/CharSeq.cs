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
using System.Runtime.Serialization;
using System.Text;
using Jenetics.Internal.Collection;
using Jenetics.Internal.Util;
using static Jenetics.Internal.Util.Require;
using Array = Jenetics.Internal.Collection.Array;

namespace Jenetics.Util
{
    [Serializable]
    public class CharSeq : CharSeqBase, IComparable<CharSeq>
    {
        protected CharSeq(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CharSeq(char[] characters) : base(Distinct((char[]) characters.Clone()))
        {
        }

        public CharSeq(string characters) : base(Distinct(ToCharArray(characters)))
        {
        }

        public int CompareTo(CharSeq set)
        {
            var result = 0;

            var n = Math.Min(Values.Length, set.Values.Length);
            for (var i = 0; i < n && result == 0; ++i)
                result = Values[i] - set.Values[i];
            if (result == 0)
                result = Values.Length - set.Values.Length;

            return result;
        }

        public static CharSeq Of(string pattern)
        {
            return new CharSeq(Expand(pattern).ToCharArray());
        }

        public CharSeq SubSequence(int start, int end)
        {
            return new CharSeq(new string(Values, start, end - start));
        }

        public static string Expand(string pattern)
        {
            var @out = new StringBuilder();

            for (int i = 0, n = pattern.Length; i < n; ++i)
                if (pattern[i] == '\\')
                {
                    ++i;
                    if (i < pattern.Length)
                        @out.Append(pattern[i]);
                }
                else if (pattern[i] == '-')
                {
                    if (i <= 0 || i >= pattern.Length - 1)
                        throw new ArgumentException(
                            $"Dangling range operator '-' near index {pattern.Length - 1}\n{pattern}");

                    var range = Expand(
                        pattern[i - 1],
                        pattern[i + 1]
                    );
                    @out.Append(range);

                    ++i;
                }
                else if (i + 1 == n || pattern[i + 1] != '-')
                {
                    @out.Append(pattern[i]);
                }

            return @out.ToString();
        }

        public static string Expand(char a, char b)
        {
            var @out = new StringBuilder();

            if (a < b)
            {
                var c = a;
                while (c <= b)
                {
                    @out.Append(c);
                    c = (char) (c + 1);
                }
            }
            else if (a > b)
            {
                var c = a;
                while (c >= b)
                {
                    @out.Append(c);
                    c = (char) (c - 1);
                }
            }

            return @out.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(ch => Equality.Eq(Values, ch.Values));
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(Values).Value;
        }

        public override string ToString()
        {
            return string.Join("", Values);
        }

        private static char[] Distinct(char[] chars)
        {
            System.Array.Sort(chars);

            var j = 0;
            for (var i = 1; i < chars.Length; ++i)
                if (chars[j] != chars[i])
                    chars[++j] = chars[i];

            var size = Math.Min(chars.Length, j + 1);
            var array = new char[size];
            System.Array.Copy(chars, 0, array, 0, size);
            return array;
        }

        private static char[] ToCharArray(string characters)
        {
            NonNull(characters, "Characters");

            return characters.ToCharArray();
        }

        public static IImmutableSeq<char> ToImmutableSeq(string chars)
        {
            var seq = MutableSeq.OfLength<char>(chars.Length);
            for (var i = 0; i < chars.Length; ++i)
                seq[i] = chars[i];

            return seq.ToImmutableSeq();
        }

        public static implicit operator string(CharSeq d)
        {
            return d.ToString();
        }
    }

    [Serializable]
    public abstract class CharSeqBase : ArrayImmutableSeq<char>, ISerializable
    {
        protected new readonly char[] Values;

        protected CharSeqBase(char[] characters) : base(Array.Of(CharStore.Of(characters)).Seal())
        {
            Values = characters;
        }

        protected CharSeqBase(SerializationInfo info, StreamingContext context) : base(Array.OfLength<char>(0))
        {
            Values = (char[]) info.GetValue("_array", typeof(char[]));
            base.Values = Array.Of(CharStore.Of(Values)).Seal();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_array", Values);
        }
    }
}