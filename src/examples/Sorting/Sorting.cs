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

namespace Jenetics.Example
{
    public static class Sorting
    {
        private static int Dist(IChromosome<EnumGene<int>> path, int i, int j)
        {
            return (path.GetGene(i).Allele - path.GetGene(j).Allele) *
                   (path.GetGene(i).Allele - path.GetGene(j).Allele);
        }

        private static int Length(Genotype<EnumGene<int>> genotype)
        {
            return Enumerable.Range(1, genotype.GetChromosome().Length - 1)
                .Select(i => Dist(genotype.GetChromosome(), i, i - 1))
                .Sum();
        }

        public static void Main()
        {
            //RandomRegistry.SetRandom(new io.jenetics.prngine.LCG64ShiftRandom.ThreadLocal());
            var engine = Engine.Engine
                .Builder(
                    Length,
                    PermutationChromosome.OfInteger(20))
                .Optimize(Optimize.Minimum)
                .PopulationSize(1000)
                .OffspringFraction(0.9)
                .Alterers(
                    new SwapMutator<EnumGene<int>, int>(0.01),
                    new PartiallyMatchedCrossover<int, int>(0.3))
                .Build();


            var statistics = EvolutionStatistics.OfNumber<int>();

            var result = engine.Stream()
                .TakeWhile(Limits.BySteadyFitness<EnumGene<int>, int>(100))
                .Take(2500)
                .Peek(statistics.Accept)
                .ToBestEvolutionResult();

            Console.WriteLine(statistics);
            Console.WriteLine(result.GetBestPhenotype());
        }
    }
}