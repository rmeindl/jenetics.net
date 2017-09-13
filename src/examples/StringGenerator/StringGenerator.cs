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
    public static class StringGenerator
    {
        private const string TargetString = "jenetics";

        private static readonly IProblem<string, CharacterGene, int> Problem =
            Engine.Problem.Of(
                seq => Enumerable.Range(0, TargetString.Length)
                    .Select(i => seq[i] == TargetString[i] ? 1 : 0)
                    .Sum(),
                Codec.Of(
                    () => Genotype.Of(new CharacterChromosome(
                        CharSeq.Of("a-z"), TargetString.Length
                    )),
                    gt => gt.GetChromosome().ToString()
                )
            );

        public static void Main()
        {
            var engine = Engine.Engine.Builder(Problem)
                .PopulationSize(500)
                .SurvivorsSelector(new StochasticUniversalSelector<CharacterGene, int>())
                .OffspringSelector(new TournamentSelector<CharacterGene, int>(5))
                .Alterers(
                    new Mutator<CharacterGene, int>(0.1),
                    new SinglePointCrossover<CharacterGene, int>(0.5))
                .Build();

            var result = engine.Stream()
                .Take(100)
                .ToBestPhenotype();

            Console.WriteLine(result);
        }
    }
}