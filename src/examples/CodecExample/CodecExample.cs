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
using Jenetics.Engine;
using Jenetics.Util;

namespace Jenetics.Example
{
    public static class CodecExample
    {
        private static double F(Tuple<int, long, double> param)
        {
            return param.Item1 + param.Item2 + param.Item3;
        }

        private static ICodec<Tuple<int, long, double>, DoubleGene> Codec(IntRange v1Domain, LongRange v2Domain, DoubleRange v3Domain)
        {
            return Engine.Codec.Of(
                () => Genotype.Of(
                    DoubleChromosome.Of(DoubleRange.Of(v1Domain.Min, v1Domain.Max)),
                    DoubleChromosome.Of(DoubleRange.Of(v2Domain.Min, v2Domain.Max)),
                    DoubleChromosome.Of(v3Domain)
                ),
                gt => Tuple.Create(
                    gt.GetChromosome(0).GetGene().IntValue(),
                    gt.GetChromosome(0).GetGene().LongValue(),
                    gt.GetChromosome(0).GetGene().DoubleValue())
            );
        }

        public static void Main()
        {
            var domain1 = IntRange.Of(0, 100);
            var domain2 = LongRange.Of(0, 1_000_000_000_000L);
            var domain3 = DoubleRange.Of(0, 1);

            var codec = Codec(domain1, domain2, domain3);

            var engine = Engine.Engine.Builder(F, codec).Build();

            var gt = engine.Stream().Take(100).ToBestGenotype();

            var param = codec.Decoder()(gt);
            Console.WriteLine($"Result:\t{param}");
        }
    }
}