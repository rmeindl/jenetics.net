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
    public static class LastMonday
    {
        private static DateTime _minMonday = new DateTime(2015, 1, 5);

        private static readonly ICodec<DateTime, AnyGene<DateTime>> Codec = Engine.Codec.Of(
            () => Genotype.Of(AnyChromosome.Of(NextRandomMonday)),
            gt => gt.Gene.Allele
        );

        private static DateTime NextRandomMonday()
        {
            return _minMonday.AddDays(RandomRegistry.GetRandom().NextInt(1000) * 7);
        }

        private static int Fitness(DateTime date)
        {
            return date.Day;
        }

        public static void Main()
        {
            var engine = Engine.Engine
                .Builder(Fitness, Codec)
                .OffspringSelector(new RouletteWheelSelector<AnyGene<DateTime>, int>())
                .Build();

            var best = engine.Stream()
                .Take(50)
                .ToBestPhenotype();

            Console.WriteLine(best);
        }
    }
}