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
using System.Threading;
using Jenetics.Engine;
using Jenetics.Util;
using static Jenetics.Engine.Limits;
using JeneticsEngine = Jenetics.Engine.Engine;

namespace Jenetics.Example;
internal static class Game
{
    public static void Main()
    {
        var codec = Codec.Of(
            () => Genotype.Of(DoubleChromosome.Of(0, 1)),
            gt => Player.Of(gt.Gene.DoubleValue())
        );

        var population = new Population<DoubleGene, double>();

        double Fitness(Player player)
        {
            var pop = Volatile.Read(ref population);

            Player other;
            if (pop.Count != 0)
            {
                var index = RandomRegistry.GetRandom().NextInt(pop.Count);
                other = codec.Decode(pop[index].GetGenotype());
            }
            else
            {
                other = Player.Of(0.5);
            }

            return player.CompareTo(other);
        }

        var engine = JeneticsEngine
            .Builder(Fitness, codec)
            .Build();

        var best = codec.Decode(
            engine.Stream()
                .TakeWhile(BySteadyFitness<DoubleGene, double>(50))
                .Peek(er => Interlocked.Exchange(ref population, er.GetPopulation()))
                .ToBestGenotype());

        Console.WriteLine(best.Value);
    }

    public class Player : IComparable<Player>
    {
        public double Value { get; }

        private Player(double value)
        {
            Value = value;
        }

        public int CompareTo(Player other)
        {
            return Value.CompareTo(other.Value);
        }

        public static Player Of(double value)
        {
            return new Player(value);
        }
    }
}