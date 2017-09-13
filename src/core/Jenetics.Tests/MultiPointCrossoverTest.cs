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
using System.Linq;
using Jenetics.Internal.Math;
using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public class MultiPointCrossoverTest : AltererTesterBase
    {
        protected override IAlterer<DoubleGene, double> NewAlterer(double p)
        {
            return new MultiPointCrossover<DoubleGene, double>(p);
        }

        [Theory]
        [MemberData(nameof(NumberOfCrossoverPoints))]
        public void ReverseCrossover(int npoints)
        {
            for (var i = 1; i < 500; ++i)
            {
                var chars = CharSeq.Of("a-zA-Z");
                var a = new CharacterChromosome(chars, i).ToSeq().Select(g => g.Allele).ToImmutableSeq();
                var b = new CharacterChromosome(chars, i).ToSeq().Select(g => g.Allele).ToImmutableSeq();

                var ma1 = a.Copy();
                var mb1 = b.Copy();
                var points = Base.Subset(
                    a.Length + 1,
                    Math.Min(npoints, a.Length + 1),
                    new Random(1234)
                );

                MultiPointCrossover.Crossover(ma1, mb1, points);
                MultiPointCrossover.Crossover(ma1, mb1, points);

                Assert.Equal(a, ma1);
                Assert.Equal(b, mb1);
            }
        }

        public static IEnumerable<object[]> NumberOfCrossoverPoints()
        {
            for (var i = 1; i < 11; i++)
                yield return new object[] {i};
        }

        [Theory]
        [MemberData(nameof(CrossoverParameters))]
        public void Crossover(string stringA, string stringB, ISeq<int> points, string expectedA, string expectedB)
        {
            var a = CharSeq.ToImmutableSeq(stringA);
            var b = CharSeq.ToImmutableSeq(stringB);

            var ma = a.Copy();
            var mb = b.Copy();

            var intPoints = points.Select(i => i).ToArray();

            MultiPointCrossover.Crossover(ma, mb, intPoints);
            Assert.Equal(ToString(ma), expectedA);
            Assert.Equal(ToString(mb), expectedB);
        }

        private static string ToString(IEnumerable<char> seq)
        {
            return string.Join("", seq);
        }

        public static IEnumerable<object[]> CrossoverParameters()
        {
            return new[]
            {
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Empty<int>(),
                    "0123456789", "ABCDEFGHIJ"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(0),
                    "ABCDEFGHIJ", "0123456789"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(1),
                    "0BCDEFGHIJ", "A123456789"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(2),
                    "01CDEFGHIJ", "AB23456789"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(3),
                    "012DEFGHIJ", "ABC3456789"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(8),
                    "01234567IJ", "ABCDEFGH89"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(9),
                    "012345678J", "ABCDEFGHI9"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(10),
                    "0123456789", "ABCDEFGHIJ"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(0, 1),
                    "A123456789", "0BCDEFGHIJ"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(3, 5),
                    "012DE56789", "ABC34FGHIJ"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(0, 1, 2),
                    "A1CDEFGHIJ", "0B23456789"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(0, 1, 2, 3),
                    "A1C3456789", "0B2DEFGHIJ"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(0, 1, 2, 3, 4),
                    "A1C3EFGHIJ", "0B2D456789"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(0, 1, 2, 3, 4, 5),
                    "A1C3E56789", "0B2D4FGHIJ"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(0, 1, 2, 3, 4, 5, 6),
                    "A1C3E5GHIJ", "0B2D4F6789"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(0, 1, 2, 3, 4, 5, 6, 7),
                    "A1C3E5G789", "0B2D4F6HIJ"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(0, 1, 2, 3, 4, 5, 6, 7, 8),
                    "A1C3E5G7IJ", "0B2D4F6H89"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(0, 1, 2, 3, 4, 5, 6, 7, 8, 9),
                    "A1C3E5G7I9", "0B2D4F6H8J"
                },
                new object[]
                {
                    "0123456789", "ABCDEFGHIJ",
                    ImmutableSeq.Of(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10),
                    "A1C3E5G7I9", "0B2D4F6H8J"
                },

                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Empty<int>(),
                    "012345678", "ABCDEFGHI"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(0),
                    "ABCDEFGHI", "012345678"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(1),
                    "0BCDEFGHI", "A12345678"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(2),
                    "01CDEFGHI", "AB2345678"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(3),
                    "012DEFGHI", "ABC345678"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(8),
                    "01234567I", "ABCDEFGH8"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(9),
                    "012345678", "ABCDEFGHI"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(0, 1),
                    "A12345678", "0BCDEFGHI"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(3, 5),
                    "012DE5678", "ABC34FGHI"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(0, 1, 2),
                    "A1CDEFGHI", "0B2345678"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(0, 1, 2, 3),
                    "A1C345678", "0B2DEFGHI"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(0, 1, 2, 3, 4),
                    "A1C3EFGHI", "0B2D45678"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(0, 1, 2, 3, 4, 5),
                    "A1C3E5678", "0B2D4FGHI"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(0, 1, 2, 3, 4, 5, 6),
                    "A1C3E5GHI", "0B2D4F678"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(0, 1, 2, 3, 4, 5, 6, 7),
                    "A1C3E5G78", "0B2D4F6HI"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(0, 1, 2, 3, 4, 5, 6, 7, 8),
                    "A1C3E5G7I", "0B2D4F6H8"
                },
                new object[]
                {
                    "012345678", "ABCDEFGHI",
                    ImmutableSeq.Of(0, 1, 2, 3, 4, 5, 6, 7, 8, 9),
                    "A1C3E5G7I", "0B2D4F6H8"
                },

                new object[]
                {
                    "0123456789", "ABCDEF",
                    ImmutableSeq.Empty<int>(),
                    "0123456789", "ABCDEF"
                },
                new object[]
                {
                    "0123456789", "ABCDEF",
                    ImmutableSeq.Of(0),
                    "ABCDEF6789", "012345"
                },
                new object[]
                {
                    "0123456789", "ABCDEF",
                    ImmutableSeq.Of(1),
                    "0BCDEF6789", "A12345"
                },
                new object[]
                {
                    "0123456789", "ABCDEF",
                    ImmutableSeq.Of(2),
                    "01CDEF6789", "AB2345"
                },
                new object[]
                {
                    "0123456789", "ABCDEF",
                    ImmutableSeq.Of(3),
                    "012DEF6789", "ABC345"
                },
                new object[]
                {
                    "0123456789", "ABCDEF",
                    ImmutableSeq.Of(5),
                    "01234F6789", "ABCDE5"
                },
                new object[]
                {
                    "0123456789", "ABCDEF",
                    ImmutableSeq.Of(6),
                    "0123456789", "ABCDEF"
                },
                new object[]
                {
                    "0123456789", "ABCDEF",
                    ImmutableSeq.Of(1, 3),
                    "0BC3456789", "A12DEF"
                }
            };
        }

        [Fact]
        public void CrossoverAll1()
        {
            var chars = CharSeq.Of("a-zA-Z");
            var g1 = new CharacterChromosome(chars, 20).ToSeq();
            var g2 = new CharacterChromosome(chars, 20).ToSeq();

            var crossover = new MultiPointCrossover<CharacterGene, double>(2000);
            var points = new int[g1.Length];
            for (var i = 0; i < points.Length; ++i)
                points[i] = i;

            var ms1 = g1.Copy();
            var ms2 = g2.Copy();

            crossover.Crossover(ms1, ms2);
        }

        [Fact]
        public void SinglePointCrossoverConsistency()
        {
            var a = CharSeq.ToImmutableSeq("1234567890");
            var b = CharSeq.ToImmutableSeq("ABCDEFGHIJ");

            for (var i = 0; i < a.Length + 1; ++i)
            {
                var ma1 = a.Copy();
                var mb1 = b.Copy();
                var ma2 = a.Copy();
                var mb2 = b.Copy();

                MultiPointCrossover.Crossover(ma1, mb1, new[] {i});
                SinglePointCrossover.Crossover(ma2, mb2, i);

                Assert.Equal(ma1, ma2);
                Assert.Equal(mb1, mb2);
            }
        }
    }
}