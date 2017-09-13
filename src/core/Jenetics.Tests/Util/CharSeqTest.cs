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
using Jenetics.Internal.Math;
using Xunit;

namespace Jenetics.Util
{
    public class CharSeqTest : ObjectTesterBase<CharSeq>
    {
        protected override Factory<CharSeq> Factory()
        {
            return () =>
            {
                var r = RandomRegistry.GetRandom();
                return new CharSeq(random.NextString(r, r.NextInt(200) + 100));
            };
        }

        [Theory]
        [ClassData(typeof(RandomStringDataGenerator))]
        public void DistinctRandomStrings(string value)
        {
            var seq = new CharSeq(value);
            var set = new HashSet<char>();
            for (var i = value.Length; --i >= 0;)
                set.Add(value[i]);

            Assert.Equal(set.Count, seq.Length);
            foreach (var c in seq)
                Assert.True(set.Contains(c), "Set must contain " + c);
        }

        private class RandomStringDataGenerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var random = new Random(123);
                for (var i = 1; i <= 25; i++)
                    yield return new object[] {Internal.Math.random.NextString(random, 25)};
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Fact]
        public void Contains()
        {
            var set = new CharSeq(CharSeq.Expand("a-z"));
            Assert.Contains('t', set);
            Assert.Contains('a', set);
            Assert.Contains('z', set);
            Assert.DoesNotContain('T', set);
            Assert.DoesNotContain('1', set);
            Assert.DoesNotContain('2', set);
        }

        [Fact]
        public void Distinct()
        {
            var cs1 = new CharSeq("abcdeaafg");
            var cs2 = new CharSeq("gfedcbabb");
            Assert.Equal(cs2, cs1);
        }

        [Fact]
        public void Distinct1()
        {
            var set = new CharSeq("".ToCharArray());
            Assert.Equal("", set.ToString());

            set = new CharSeq("1".ToCharArray());
            Assert.Equal("1", set.ToString());

            set = new CharSeq("11".ToCharArray());
            Assert.Equal("1", set.ToString());

            set = new CharSeq("142321423456789".ToCharArray());
            Assert.Equal("123456789", set.ToString());

            set = new CharSeq("0000000000000000000000000".ToCharArray());
            Assert.Equal("0", set.ToString());

            set = new CharSeq("0111111111111111111111111112".ToCharArray());
            Assert.Equal("012", set.ToString());

            set = new CharSeq("111111111111111112".ToCharArray());
            Assert.Equal("12", set.ToString());

            set = new CharSeq("1222222222222222222".ToCharArray());
            Assert.Equal("12", set.ToString());

            set = new CharSeq("000000987654321111111111".ToCharArray());
            Assert.Equal("0123456789", set.ToString());
        }

        [Fact]
        public void Distinct2()
        {
            var set = new CharSeq("");
            Assert.Equal("", set.ToString());

            set = new CharSeq("1");
            Assert.Equal("1", set.ToString());

            set = new CharSeq("11");
            Assert.Equal("1", set.ToString());

            set = new CharSeq("1223345667899");
            Assert.Equal("123456789", set.ToString());

            set = new CharSeq("0000000000000000000000000");
            Assert.Equal("0", set.ToString());

            set = new CharSeq("0111111111111111111111111112");
            Assert.Equal("012", set.ToString());

            set = new CharSeq("111111111111111112");
            Assert.Equal("12", set.ToString());

            set = new CharSeq("1222222222222222222");
            Assert.Equal("12", set.ToString());

            set = new CharSeq("000000987654321111111111");
            Assert.Equal("0123456789", set.ToString());
        }

        [Fact]
        public void Expand1()
        {
            var value = CharSeq.Expand('a', 'z');
            Assert.Equal(26, value.Length);
            Assert.Equal("abcdefghijklmnopqrstuvwxyz", value);

            value = CharSeq.Expand('A', 'Z');
            Assert.Equal(26, value.Length);
            Assert.Equal("ABCDEFGHIJKLMNOPQRSTUVWXYZ", value);

            value = CharSeq.Expand('0', '9');
            Assert.Equal(10, value.Length);
            Assert.Equal("0123456789", value);
        }

        [Fact]
        public void Expand2()
        {
            var value = CharSeq.Expand("a-z");
            Assert.Equal(26, value.Length);
            Assert.Equal("abcdefghijklmnopqrstuvwxyz", value);

            value = CharSeq.Expand("a-z\\-");
            Assert.Equal(27, value.Length);
            Assert.Equal("abcdefghijklmnopqrstuvwxyz-", value);

            value = CharSeq.Expand("a-z\\\\xx");
            Assert.Equal(29, value.Length);
            Assert.Equal("abcdefghijklmnopqrstuvwxyz\\xx", value);

            value = CharSeq.Expand("A-Z");
            Assert.Equal(26, value.Length);
            Assert.Equal("ABCDEFGHIJKLMNOPQRSTUVWXYZ", value);

            value = CharSeq.Expand("0-9");
            Assert.Equal(10, value.Length);
            Assert.Equal("0123456789", value);

            value = CharSeq.Expand("0-9yxcvba-z");
            Assert.Equal(41, value.Length);
            Assert.Equal("0123456789yxcvbabcdefghijklmnopqrstuvwxyz", value);

            value = CharSeq.Expand("0-9a-zA-Z");
            Assert.Equal(10 + 26 + 26, value.Length);
            Assert.Equal("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", value);
        }

        [Fact]
        public void Expand3()
        {
            Assert.Throws<ArgumentException>(() => CharSeq.Expand("a-z-"));
        }

        [Fact]
        public void Expand4()
        {
            Assert.Throws<ArgumentException>(() => CharSeq.Expand("-az"));
        }

        [Fact]
        public void Iterate()
        {
            var set = new CharSeq(CharSeq.Expand("a-z"));
            var values = CharSeq.Expand("a-z");
            using (var it = set.GetEnumerator())
            {
                foreach (var t in values)
                {
                    Assert.True(it.MoveNext());
                    Assert.Equal(t, it.Current);
                }
                Assert.False(it.MoveNext());
            }
        }

        [Fact]
        public void SubSequence()
        {
            var set = new CharSeq(CharSeq.Expand("a-z"));
            var sub = set.SubSequence(3, 10);
            Assert.Equal(7, sub.Length);
            Assert.Equal("defghij", sub.ToString());
        }
    }
}