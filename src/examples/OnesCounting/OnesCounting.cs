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
using static Jenetics.Engine.Limits;

namespace Jenetics.Example
{
    public static class OnesCounting
    {
        private static int Count(Genotype<BitGene> gt)
        {
            return ((BitChromosome) gt.GetChromosome()).BitCount();
        }

        public static void Main()
        {
            var engine = Engine.Engine
                .Builder(
                    Count,
                    BitChromosome.Of(20, 0.15))
                .PopulationSize(500)
                .Selector(new RouletteWheelSelector<BitGene, int>())
                .Alterers(
                    new Mutator<BitGene, int>(0.55),
                    new SinglePointCrossover<BitGene, int>(0.06))
                .Build();

            var statistics = EvolutionStatistics.OfNumber<int>();

            var best = engine.Stream()
                .TakeWhile(BySteadyFitness<BitGene, int>(7))
                .Take(100)
                .Peek(statistics.Accept)
                .ToBestPhenotype();

            Console.WriteLine(statistics);
            Console.WriteLine(best);
        }
    }
}