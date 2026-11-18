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
using JeneticsEngine = Jenetics.Engine.Engine;

namespace Jenetics.Example;
public static class CodecExample
{
    private static double Fitness((int IntegerValue, long LongValue, double DoubleValue) values)
    {
        return values.IntegerValue + values.LongValue + values.DoubleValue;
    }

    private static ICodec<(int IntegerValue, long LongValue, double DoubleValue), DoubleGene> CreateCodec(
        IntRange integerDomain,
        LongRange longDomain,
        DoubleRange doubleDomain
    )
    {
        return Codec.Of(
            () => Genotype.Of(
                DoubleChromosome.Of(DoubleRange.Of(integerDomain.Min, integerDomain.Max)),
                DoubleChromosome.Of(DoubleRange.Of(longDomain.Min, longDomain.Max)),
                DoubleChromosome.Of(doubleDomain)
            ),
            gt => (
                gt.GetChromosome(0).GetGene().IntValue(),
                gt.GetChromosome(1).GetGene().LongValue(),
                gt.GetChromosome(2).GetGene().DoubleValue()
            )
        );
    }

    public static void Main()
    {
        var domain1 = IntRange.Of(0, 100);
        var domain2 = LongRange.Of(0, 1_000_000_000_000L);
        var domain3 = DoubleRange.Of(0, 1);

        var codec = CreateCodec(domain1, domain2, domain3);

        var engine = JeneticsEngine.Builder(Fitness, codec).Build();

        var gt = engine.Stream().Take(100).ToBestGenotype();

        var param = codec.Decoder()(gt);
        Console.WriteLine($"Result:\t{param}");
    }
}