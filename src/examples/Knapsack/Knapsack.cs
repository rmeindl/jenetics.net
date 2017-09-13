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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jenetics.Engine;
using Jenetics.Internal.Util;
using Jenetics.Util;
using static Jenetics.Engine.Limits;

namespace Jenetics.Example
{
    public class Knapsack : IProblem<IImmutableSeq<Knapsack.Item>, BitGene, double>
    {
        private readonly ICodec<IImmutableSeq<Item>, BitGene> _codec;
        private readonly double _knapsackSize;

        private Knapsack(IImmutableSeq<Item> items, double knapsackSize)
        {
            _codec = Codecs.OfSubSet(items);
            _knapsackSize = knapsackSize;
        }

        public Func<IImmutableSeq<Item>, double> Fitness()
        {
            return items =>
            {
                var sumSize = 0.0;
                var sumValue = 0.0;
                foreach (var item in items)
                {
                    sumSize += item.GetSize();
                    sumValue += item.GetValue();
                }
                return sumSize <= _knapsackSize ? sumValue : 0;
            };
        }

        public ICodec<IImmutableSeq<Item>, BitGene> Codec()
        {
            return _codec;
        }

        private static Knapsack Of(int itemCount, Random random)
        {
            IEnumerable<Item> F()
            {
                for (var i = 0; i < itemCount; i++)
                    yield return Item.Random(random);
            }

            return new Knapsack(F().ToImmutableSeq(), itemCount * 100.0 / 3.0);
        }

        public static void Main()
        {
            var knapsack = Of(15, new Random(123));

            var engine = Engine.Engine.Builder(knapsack)
                .PopulationSize(500)
                .SurvivorsSelector(new TournamentSelector<BitGene, double>(5))
                .OffspringSelector(new RouletteWheelSelector<BitGene, double>())
                .Alterers(
                    new Mutator<BitGene, double>(0.115),
                    new SinglePointCrossover<BitGene, double>(0.16))
                .Build();

            var statistics = EvolutionStatistics.OfNumber<double>();

            var best = engine.Stream()
                .TakeWhile(BySteadyFitness<BitGene, double>(7))
                .Take(100)
                .Peek(statistics.Accept)
                .ToBestPhenotype();

            Console.WriteLine(statistics);
            Console.WriteLine(best);
        }

        public class Item
        {
            private readonly double _size;
            private readonly double _value;

            private Item(double size, double value)
            {
                _size = Require.NonNegative(size);
                _value = Require.NonNegative(value);
            }

            public double GetSize()
            {
                return _size;
            }

            public double GetValue()
            {
                return _value;
            }

            public override bool Equals(object obj)
            {
                return obj is Item item &&
                       Comparer.Default.Compare(_size, item._size) == 0 &&
                       Comparer.Default.Compare(_value, item._value) == 0;
            }

            public override int GetHashCode()
            {
                var hash = 1;
                var bits = (ulong) BitConverter.DoubleToInt64Bits(_size);
                hash = 31 * hash + (int) (bits ^ (bits >> 32));

                bits = (ulong) BitConverter.DoubleToInt64Bits(_value);
                return 31 * hash + (int) (bits ^ (bits >> 32));
            }

            public override string ToString()
            {
                return $"Item[size={_size}, value={_value}]";
            }

            public static Item Random(Random random)
            {
                var item = new Item(random.NextDouble() * 100, random.NextDouble() * 100);
                return item;
            }
        }
    }
}