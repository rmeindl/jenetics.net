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

namespace Jenetics.Engine
{
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
}