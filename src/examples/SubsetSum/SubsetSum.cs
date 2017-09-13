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
using static Jenetics.Internal.Util.Require;

namespace Jenetics.Example
{
    public class SubsetSum : IProblem<IImmutableSeq<int>, EnumGene<int>, int>
    {
        private readonly IImmutableSeq<int> _basicSet;
        private readonly int _size;

        private SubsetSum(IImmutableSeq<int> basicSet, int size)
        {
            _basicSet = NonNull(basicSet);
            _size = size;
        }

        public Func<IImmutableSeq<int>, int> Fitness()
        {
            return subset => Math.Abs(
                subset.Sum()
            );
        }

        public ICodec<IImmutableSeq<int>, EnumGene<int>> Codec()
        {
            return Codecs.OfSubSet(_basicSet, _size);
        }

        private static SubsetSum Of(int n, int k, Random random)
        {
            return new SubsetSum(
                random.Doubles()
                    .Take(n)
                    .Select(d => (int) ((d - 0.5) * n))
                    .ToImmutableSeq(),
                k
            );
        }

        public static void Main()
        {
            //SubsetSum problem = Of(500, 15, new LCG64ShiftRandom(101010));
            var problem = Of(500, 15, new Random());

            var engine = Engine.Engine.Builder(problem)
                .Minimizing()
                .MaximalPhenotypeAge(5)
                .Alterers(
                    new PartiallyMatchedCrossover<int, int>(0.4),
                    new Mutator<EnumGene<int>, int>(0.3))
                .Build();

            var result = engine.Stream()
                .TakeWhile(Limits.BySteadyFitness<EnumGene<int>, int>(55))
                .ToBestPhenotype();

            Console.WriteLine(result);
        }
    }
}