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
using System.Linq;
using Jenetics.Util;
using Xunit;

namespace Jenetics.Engine;

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

    [Fact]
    public void OfIntVector()
    {
        var codec = Codecs.OfVector(IntRange.Of(-10, 10), 4);
        var genotype = codec.Encoding()();

        Assert.Equal(1, genotype.Length);
        Assert.Equal(4, genotype.GetChromosome().Length);
        Assert.Equal(
            genotype.GetChromosome().Select(gene => gene.Allele).ToArray(),
            codec.Decode(genotype)
        );
    }

    [Fact]
    public void OfIntVectorWithIndividualDomains()
    {
        var domains = new[]
        {
            IntRange.Of(0, 1),
            IntRange.Of(10, 20),
            IntRange.Of(-20, -10),
        };
        var codec = Codecs.OfVector(domains);
        var genotype = codec.Encoding()();

        Assert.Equal(domains.Length, genotype.Length);
        var values = codec.Decode(genotype);
        for (var i = 0; i < domains.Length; ++i)
        {
            Assert.InRange(values[i], domains[i].Min, domains[i].Max);
            Assert.Equal(values[i], genotype.GetChromosome(i).GetGene().Allele);
        }
    }

    [Fact]
    public void OfVectorWithSequenceValidator()
    {
        var codec = Codecs.OfVector(
            () => 7,
            value => value > 0,
            values => values.All(value => value == 7),
            3
        );

        Assert.All(codec.Decode(codec.Encoding()()), value => Assert.Equal(7, value));
    }

    [Fact]
    public void OfPermutationWithArbitraryAlleles()
    {
        var codec = Codecs.OfPermutation("a", "b", "c", "d");
        var genotype = codec.Encoding()();
        var decoded = codec.Decode(genotype);

        Assert.Equal(4, decoded.Length);
        Assert.Equal(new[] { "a", "b", "c", "d" }, decoded.OrderBy(value => value));
    }

    [Fact]
    public void CombineCodecsWithDifferentChromosomeStructures()
    {
        var vector = Codecs.OfVector(IntRange.Of(0, 100), 3);
        var scalar = Codecs.OfScalar(IntRange.Of(0, 100));
        var codec = Codec.Combine(vector, scalar, (values, value) => (Values: values, Value: value));
        var genotype = codec.Encoding()();
        var decoded = codec.Decode(genotype);

        Assert.Equal(2, genotype.Length);
        Assert.Equal(
            genotype.GetChromosome(0).Select(gene => gene.Allele).ToArray(),
            decoded.Values
        );
        Assert.Equal(genotype.GetChromosome(1).GetGene().Allele, decoded.Value);
    }

    [Fact]
    public void CombineArbitraryNumberOfCodecs()
    {
        ICodec<object, IntegerGene>[] codecs =
        {
            Box(Codecs.OfScalar(IntRange.Of(0, 10))),
            Box(Codecs.OfScalar(IntRange.Of(0, 10))),
            Box(Codecs.OfScalar(IntRange.Of(0, 10))),
            Box(Codecs.OfScalar(IntRange.Of(0, 10))),
        };
        var codec = Codec.Combine<IntegerGene, int>(
            values => values.Sum(value => (int)value!),
            codecs
        );
        var genotype = codec.Encoding()();

        Assert.Equal(4, genotype.Length);
        Assert.Equal(
            genotype.ToSeq().Sum(chromosome => chromosome.GetGene().Allele),
            codec.Decode(genotype)
        );
    }

    private static ICodec<object, TGene> Box<T, TGene>(ICodec<T, TGene> codec)
        where TGene : IGene<TGene>
    {
        return Codec.Of<object, TGene>(codec.Encoding(), genotype => codec.Decode(genotype)!);
    }

    public static IEnumerable<object[]> IntScalarData()
    {
        yield return new object[] { IntRange.Of(0, 1) };
        yield return new object[] { IntRange.Of(0, 10) };
        yield return new object[] { IntRange.Of(1, 2) };
        yield return new object[] { IntRange.Of(0, 100) };
        yield return new object[] { IntRange.Of(10, 1000) };
        yield return new object[] { IntRange.Of(1000, 10000) };
    }

    public static IEnumerable<object[]> LongScalarData()
    {
        yield return new object[] { LongRange.Of(0, 1) };
        yield return new object[] { LongRange.Of(0, 10) };
        yield return new object[] { LongRange.Of(1, 2) };
        yield return new object[] { LongRange.Of(0, 100) };
        yield return new object[] { LongRange.Of(10, 1000) };
        yield return new object[] { LongRange.Of(1000, 10000) };
    }

    public static IEnumerable<object[]> DoubleScalarData()
    {
        yield return new object[] { DoubleRange.Of(0, 1) };
        yield return new object[] { DoubleRange.Of(0, 10) };
        yield return new object[] { DoubleRange.Of(1, 2) };
        yield return new object[] { DoubleRange.Of(0, 100) };
        yield return new object[] { DoubleRange.Of(10, 1000) };
        yield return new object[] { DoubleRange.Of(1000, 10000) };
    }
}