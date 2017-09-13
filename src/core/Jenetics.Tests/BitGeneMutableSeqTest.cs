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
using System.Collections.Generic;
using Jenetics.Util;
using Xunit;
using Array = Jenetics.Internal.Collection.Array;

namespace Jenetics
{
    public class BitGeneMutableSeqTest
    {
        public static IMutableSeq<BitGene> NewSeq(int length)
        {
            return BitGeneMutableSeq.Of(Array.Of(BitGeneStore.OfLength(length)));
        }

        [Theory]
        [MemberData(nameof(Sequences))]
        public void SwapIntInt(IMutableSeq<BitGene> seq)
        {
            for (var i = 0; i < seq.Length - 3; ++i)
            {
                var copy = seq.ToArray(new BitGene[0]);
                var j = i + 2;
                var vi = seq[i];
                var vj = seq[j];

                seq.Swap(i, j);

                Assert.Equal(vj, seq[i]);
                Assert.Equal(vi, seq[j]);
                for (var k = 0; k < seq.Length; ++k)
                    if (k != i && k != j)
                        Assert.Equal(copy[k], seq[k]);
            }
        }

        [Theory]
        [MemberData(nameof(Sequences))]
        public void SwapIntIntMSeqInt(IMutableSeq<BitGene> seq)
        {
            for (var start = 0; start < seq.Length - 3; ++start)
            {
                var random = new Random();
                var other = NewSeq(seq.Length);
                var otherCopy = NewSeq(seq.Length);
                for (var j = 0; j < other.Length; ++j)
                {
                    other[j] = BitGene.Of(random.NextBoolean());
                    otherCopy[j] = other[j];
                }

                var copy = seq.ToArray(new BitGene[0]);
                var end = start + 2;
                const int otherStart = 1;

                seq.Swap(start, end, other, otherStart);

                for (var j = start; j < end; ++j)
                    Assert.Equal(seq[j], otherCopy[j + otherStart - start]);
                for (var j = 0; j < end - start; ++j)
                    Assert.Equal(other[j + otherStart], copy[j + start]);
            }
        }

        public static IEnumerable<object[]> Sequences()
        {
            yield return new object[] {NewSeq(330)};
            yield return new object[] {NewSeq(350).SubSeq(50)};
            yield return new object[] {NewSeq(330).SubSeq(80, 230)};
            yield return new object[] {NewSeq(500).SubSeq(50, 430).SubSeq(100)};
        }
    }
}