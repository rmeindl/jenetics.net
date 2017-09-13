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

using System.Collections.Generic;
using Jenetics.Util;
using Xunit;

namespace Jenetics.Engine
{
    public class CodecsTest
    {
        [Theory]
        [MemberData(nameof(IntScalarData))]
        public void OfIntScalar(IntRange domain)
        {
            var codec = Codecs.OfScalar(domain);

            var gt = codec.Encoding()();
            Assert.Equal(1, gt.Length);
            Assert.Equal(1, gt.GetChromosome().Length);
            Assert.Equal(domain.Min, gt.Gene.Min);
            Assert.Equal(domain.Max, gt.Gene.Max);

            var f = codec.Decoder();
            Assert.Equal(gt.Gene.IntValue(), f(gt));
        }

        [Theory]
        [MemberData(nameof(LongScalarData))]
        public void OfLongScalar(LongRange domain)
        {
            var codec = Codecs.OfScalar(domain);

            var gt = codec.Encoding()();
            Assert.Equal(1, gt.Length);
            Assert.Equal(1, gt.GetChromosome().Length);
            Assert.Equal(domain.Min, gt.Gene.Min);
            Assert.Equal(domain.Max, gt.Gene.Max);

            var f = codec.Decoder();
            Assert.Equal(gt.Gene.LongValue(), f(gt));
        }

        [Theory]
        [MemberData(nameof(DoubleScalarData))]
        public void OfDoubleScalar(DoubleRange domain)
        {
            var codec = Codecs.OfScalar(domain);

            var gt = codec.Encoding()();
            Assert.Equal(1, gt.Length);
            Assert.Equal(1, gt.GetChromosome().Length);
            Assert.Equal(domain.Min, gt.Gene.Min);
            Assert.Equal(domain.Max, gt.Gene.Max);

            var f = codec.Decoder();
            Assert.Equal(gt.Gene.DoubleValue(), f(gt));
        }

        public static IEnumerable<object[]> IntScalarData()
        {
            yield return new object[] {IntRange.Of(0, 1)};
            yield return new object[] {IntRange.Of(0, 10)};
            yield return new object[] {IntRange.Of(1, 2)};
            yield return new object[] {IntRange.Of(0, 100)};
            yield return new object[] {IntRange.Of(10, 1000)};
            yield return new object[] {IntRange.Of(1000, 10000)};
        }

        public static IEnumerable<object[]> LongScalarData()
        {
            yield return new object[] {LongRange.Of(0, 1)};
            yield return new object[] {LongRange.Of(0, 10)};
            yield return new object[] {LongRange.Of(1, 2)};
            yield return new object[] {LongRange.Of(0, 100)};
            yield return new object[] {LongRange.Of(10, 1000)};
            yield return new object[] {LongRange.Of(1000, 10000)};
        }

        public static IEnumerable<object[]> DoubleScalarData()
        {
            yield return new object[] {DoubleRange.Of(0, 1)};
            yield return new object[] {DoubleRange.Of(0, 10)};
            yield return new object[] {DoubleRange.Of(1, 2)};
            yield return new object[] {DoubleRange.Of(0, 100)};
            yield return new object[] {DoubleRange.Of(10, 1000)};
            yield return new object[] {DoubleRange.Of(1000, 10000)};
        }
    }
}