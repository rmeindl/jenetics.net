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

namespace Jenetics.Engine;

public interface ICodec<out T, TGene>
    where TGene : IGene<TGene>
{
    Factory<Genotype<TGene>> Encoding();
    Func<Genotype<TGene>, T> Decoder();
}

public static class Codec
{
    public static ICodec<T, TGene> Of<T, TGene>(Factory<Genotype<TGene>> encoding,
        Func<Genotype<TGene>, T> decoder) where TGene : IGene<TGene>
    {
        return new IntCodec<T, TGene>(encoding, decoder);
    }

    public static T Decode<T, TGene>(this ICodec<T, TGene> codec, Genotype<TGene> gt)
        where TGene : IGene<TGene>
    {
        return codec.Decoder()(gt);
    }

    public static ICodec<TResult, TGene> Combine<TFirst, TSecond, TResult, TGene>(
        ICodec<TFirst, TGene> first,
        ICodec<TSecond, TGene> second,
        Func<TFirst, TSecond, TResult> decoder
    )
        where TGene : IGene<TGene>
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(decoder);

        return Combine(
            new ICodecPart<TGene>[]
            {
                new CodecPart<TFirst, TGene>(first),
                new CodecPart<TSecond, TGene>(second),
            },
            values => decoder((TFirst)values[0]!, (TSecond)values[1]!)
        );
    }

    public static ICodec<TResult, TGene> Combine<TFirst, TSecond, TThird, TResult, TGene>(
        ICodec<TFirst, TGene> first,
        ICodec<TSecond, TGene> second,
        ICodec<TThird, TGene> third,
        Func<TFirst, TSecond, TThird, TResult> decoder
    )
        where TGene : IGene<TGene>
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(third);
        ArgumentNullException.ThrowIfNull(decoder);

        return Combine(
            new ICodecPart<TGene>[]
            {
                new CodecPart<TFirst, TGene>(first),
                new CodecPart<TSecond, TGene>(second),
                new CodecPart<TThird, TGene>(third),
            },
            values => decoder(
                (TFirst)values[0]!,
                (TSecond)values[1]!,
                (TThird)values[2]!
            )
        );
    }

    public static ICodec<TResult, TGene> Combine<TGene, TResult>(
        Func<object?[], TResult> decoder,
        params ICodec<object, TGene>[] codecs
    )
        where TGene : IGene<TGene>
    {
        ArgumentNullException.ThrowIfNull(decoder);
        ArgumentNullException.ThrowIfNull(codecs);
        if (codecs.Length == 0)
            throw new ArgumentException("At least one codec is required.", nameof(codecs));

        return Combine(
            codecs.Select(codec =>
            {
                ArgumentNullException.ThrowIfNull(codec);
                return (ICodecPart<TGene>)new CodecPart<object, TGene>(codec);
            }).ToArray(),
            decoder
        );
    }

    private static ICodec<TResult, TGene> Combine<TResult, TGene>(
        ICodecPart<TGene>[] codecs,
        Func<object?[], TResult> decoder
    )
        where TGene : IGene<TGene>
    {
        var templates = codecs.Select(codec => codec.Encoding()()).ToArray();
        var chromosomeCounts = templates.Select(genotype => genotype.Length).ToArray();

        Factory<Genotype<TGene>> encoding = () =>
            Genotype.Of(templates.SelectMany(genotype => genotype.NewInstance()));

        return Of(encoding, genotype =>
        {
            var values = new object?[codecs.Length];
            var start = 0;
            for (var i = 0; i < codecs.Length; ++i)
            {
                var part = Genotype.Of(
                    genotype.ToSeq().Skip(start).Take(chromosomeCounts[i])
                );
                values[i] = codecs[i].Decode(part);
                start += chromosomeCounts[i];
            }

            return decoder(values);
        });
    }

    private interface ICodecPart<TGene>
        where TGene : IGene<TGene>
    {
        Factory<Genotype<TGene>> Encoding();
        object? Decode(Genotype<TGene> genotype);
    }

    private sealed class CodecPart<T, TGene> : ICodecPart<TGene>
        where TGene : IGene<TGene>
    {
        private readonly ICodec<T, TGene> _codec;

        public CodecPart(ICodec<T, TGene> codec)
        {
            _codec = codec;
        }

        public Factory<Genotype<TGene>> Encoding()
        {
            return _codec.Encoding();
        }

        public object? Decode(Genotype<TGene> genotype)
        {
            return _codec.Decode(genotype);
        }
    }

    private class IntCodec<T, TGene> : ICodec<T, TGene>
        where TGene : IGene<TGene>
    {
        private readonly Func<Genotype<TGene>, T> _decoder;
        private readonly Factory<Genotype<TGene>> _encoding;

        public IntCodec(Factory<Genotype<TGene>> encoding, Func<Genotype<TGene>, T> decoder)
        {
            _encoding = encoding;
            _decoder = decoder;
        }

        public Func<Genotype<TGene>, T> Decoder()
        {
            return _decoder;
        }

        public Factory<Genotype<TGene>> Encoding()
        {
            return _encoding;
        }
    }
}