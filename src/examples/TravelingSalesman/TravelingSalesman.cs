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
    public static class TravelingSalesman
    {
        private const int Stops = 20;
        private static readonly double[,] Adjacence = Matrix(Stops);

        private static double[,] Matrix(int stops)
        {
            const double radius = 10.0;
            var matrix = new double[stops, stops];

            for (var i = 0; i < stops; ++i)
            for (var j = 0; j < stops; ++j)
                matrix[i, j] = Chord(stops, Math.Abs(i - j), radius);
            return matrix;
        }

        private static double Chord(int stops, int i, double r)
        {
            return 2.0 * r * Math.Abs(Math.Sin(Math.PI * i / stops));
        }

        private static double Dist(int[] path)
        {
            return Enumerable.Range(0, Stops)
                .Select(i => Adjacence[path[i], path[(i + 1) % Stops]])
                .Sum();
        }

        public static void Main()
        {
            var engine = Engine.Engine
                .Builder(Dist, Codecs.OfPermutation(Stops))
                .Optimize(Optimize.Minimum)
                .MaximalPhenotypeAge(11)
                .PopulationSize(500)
                .Alterers(
                    new SwapMutator<EnumGene<int>, double>(0.2),
                    new PartiallyMatchedCrossover<int, double>(0.35)
                )
                .Build();

            var statistics = EvolutionStatistics.OfNumber<double>();

            var best = engine.Stream()
                .TakeWhile(BySteadyFitness<EnumGene<int>, double>(15))
                .Take(250)
                .Peek(statistics.Accept)
                .ToBestPhenotype();

            Console.WriteLine(statistics);
            Console.WriteLine(best);
        }
    }
}