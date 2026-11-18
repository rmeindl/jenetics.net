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
using System.Linq;
using Jenetics.Internal.Math;
using Jenetics.Util;
using static Jenetics.Internal.Util.Require;

namespace Jenetics.Engine;

public static class Codecs
{
    public static ICodec<double, DoubleGene> OfScalar(DoubleRange domain)
    {
        NonNull(domain);
        return Codec.Of(
            () => Genotype.Of(DoubleChromosome.Of(domain)),
            gt => gt.GetChromosome().GetGene().Allele
        );
    }

    public static ICodec<int, IntegerGene> OfScalar(IntRange domain)
    {
        NonNull(domain);

        return Codec.Of(
            () => Genotype.Of(IntegerChromosome.Of(domain)),
            gt => gt.GetChromosome().GetGene().Allele
        );
    }

    public static ICodec<long, LongGene> OfScalar(LongRange domain)
    {
        NonNull(domain);

        return Codec.Of(
            () => Genotype.Of(LongChromosome.Of(domain)),
            gt => gt.GetChromosome().GetGene().Allele
        );
    }

    public static ICodec<TAllele, AnyGene<TAllele>> OfScalar<TAllele>(
        Func<TAllele> supplier
    )
    {
        NonNull(supplier);
        return Codec.Of(
            () => Genotype.Of(AnyChromosome.Of(supplier)),
            gt => gt.Gene.Allele
        );
    }

    public static ICodec<TAllele, AnyGene<TAllele>> OfScalar<TAllele>(
        Func<TAllele> supplier,
        Func<TAllele, bool> validator
    )
    {
        NonNull(supplier);
        NonNull(validator);
        return Codec.Of(
            () => Genotype.Of(AnyChromosome.Of(supplier, validator, 1)),
            gt => gt.Gene.Allele
        );
    }

    public static ICodec<int[], IntegerGene> OfVector(IntRange domain, int length)
    {
        NonNull(domain);
        Positive(length);
        return Codec.Of(
            () => Genotype.Of(IntegerChromosome.Of(domain.Min, domain.Max, length)),
            gt => gt.GetChromosome().Select(gene => gene.Allele).ToArray()
        );
    }

    public static ICodec<long[], LongGene> OfVector(LongRange domain, int length)
    {
        NonNull(domain);
        Positive(length);
        return Codec.Of(
            () => Genotype.Of(LongChromosome.Of(domain.Min, domain.Max, length)),
            gt => gt.GetChromosome().Select(gene => gene.Allele).ToArray()
        );
    }

    public static ICodec<double[], DoubleGene> OfVector(DoubleRange domain, int length)
    {
        NonNull(domain);
        Positive(length);
        return Codec.Of(
            () => Genotype.Of(DoubleChromosome.Of(domain.Min, domain.Max, length)),
            gt => gt.GetChromosome().Select(gene => gene.Allele).ToArray()
        );
    }

    public static ICodec<int[], IntegerGene> OfVector(params IntRange[] domains)
    {
        NonNull(domains);
        if (domains.Length == 0)
            throw new ArgumentException("Domains must not be empty.", nameof(domains));
        foreach (var domain in domains)
            NonNull(domain);

        var ranges = domains.ToArray();
        return Codec.Of(
            () => Genotype.Of(ranges.Select(domain =>
                (IChromosome<IntegerGene>)IntegerChromosome.Of(domain.Min, domain.Max))),
            gt => ranges.Select((_, index) => gt.GetChromosome(index).GetGene().Allele).ToArray()
        );
    }

    public static ICodec<long[], LongGene> OfVector(params LongRange[] domains)
    {
        NonNull(domains);
        if (domains.Length == 0)
            throw new ArgumentException("Domains must not be empty.", nameof(domains));
        foreach (var domain in domains)
            NonNull(domain);

        var ranges = domains.ToArray();
        return Codec.Of(
            () => Genotype.Of(ranges.Select(domain =>
                (IChromosome<LongGene>)new LongChromosome(domain.Min, domain.Max))),
            gt => ranges.Select((_, index) => gt.GetChromosome(index).GetGene().Allele).ToArray()
        );
    }

    public static ICodec<double[], DoubleGene> OfVector(params DoubleRange[] domains)
    {
        NonNull(domains);
        if (domains.Length == 0)
            throw new ArgumentException("Domains must not be empty.", nameof(domains));
        foreach (var domain in domains)
            NonNull(domain);

        var ranges = domains.ToArray();
        return Codec.Of(
            () => Genotype.Of(ranges.Select(domain =>
                (IChromosome<DoubleGene>)DoubleChromosome.Of(domain.Min, domain.Max))),
            gt => ranges.Select((_, index) => gt.GetChromosome(index).GetGene().Allele).ToArray()
        );
    }

    public static ICodec<IImmutableSeq<TAllele>, AnyGene<TAllele>> OfVector<TAllele>(
        Func<TAllele> supplier,
        int length
    )
    {
        NonNull(supplier);
        Positive(length);
        return Codec.Of(
            () => Genotype.Of(AnyChromosome.Of(supplier, length)),
            gt => gt.GetChromosome().Select(gene => gene.Allele).ToImmutableSeq()
        );
    }

    public static ICodec<IImmutableSeq<TAllele>, AnyGene<TAllele>> OfVector<TAllele>(
        Func<TAllele> supplier,
        Func<TAllele, bool> validator,
        int length
    )
    {
        NonNull(supplier);
        NonNull(validator);
        Positive(length);
        return Codec.Of(
            () => Genotype.Of(AnyChromosome.Of(supplier, validator, length)),
            gt => gt.GetChromosome().Select(gene => gene.Allele).ToImmutableSeq()
        );
    }

    public static ICodec<IImmutableSeq<TAllele>, AnyGene<TAllele>> OfVector<TAllele>(
        Func<TAllele> supplier,
        Func<TAllele, bool> alleleValidator,
        Func<IImmutableSeq<TAllele>, bool> alleleSeqValidator,
        int length
    )
    {
        NonNull(supplier);
        NonNull(alleleValidator);
        NonNull(alleleSeqValidator);
        Positive(length);
        return Codec.Of(
            () => Genotype.Of(AnyChromosome.Of(
                supplier,
                alleleValidator,
                alleleSeqValidator,
                length
            )),
            gt => gt.GetChromosome().Select(gene => gene.Allele).ToImmutableSeq()
        );
    }

    public static ICodec<IImmutableSeq<T>, BitGene> OfSubSet<T>(IImmutableSeq<T> basicSet)
    {
        NonNull(basicSet);
        Positive(basicSet.Length);

        return Codec.Of(
            () => Genotype.Of(BitChromosome.Of(basicSet.Length)),
            gt => ((BitChromosome)gt.GetChromosome()).Ones().Select(i => basicSet[i]).ToImmutableSeq()
        );
    }

    public static ICodec<IImmutableSeq<T>, EnumGene<T>> OfSubSet<T>(
        IImmutableSeq<T> basicSet,
        int size
    )
    {
        NonNull(basicSet);
        Base.CheckSubSet(basicSet.Length, size);

        return Codec.Of(
            () => Genotype.Of(PermutationChromosome.Of(basicSet, size)),
            gt => gt.GetChromosome()
                .Select(g => g.Allele)
                .ToImmutableSeq());
    }

    public static ICodec<int[], EnumGene<int>> OfPermutation(int length)
    {
        Positive(length);

        return Codec.Of(
            () => Genotype.Of(PermutationChromosome.OfInteger(length)),
            gt => gt.GetChromosome().ToSeq()
                .Select(g => g.Allele)
                .ToArray()
        );
    }

    public static ICodec<IImmutableSeq<T>, EnumGene<T>> OfPermutation<T>(IImmutableSeq<T> alleles)
    {
        NonNull(alleles);
        Positive(alleles.Length);
        return Codec.Of(
            () => Genotype.Of(PermutationChromosome.Of(alleles)),
            gt => gt.GetChromosome().Select(gene => gene.Allele).ToImmutableSeq()
        );
    }

    public static ICodec<IImmutableSeq<T>, EnumGene<T>> OfPermutation<T>(params T[] alleles)
    {
        NonNull(alleles);
        return OfPermutation(ImmutableSeq.Of(alleles));
    }
}