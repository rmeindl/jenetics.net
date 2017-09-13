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
using Jenetics.Internal.Util;
using Jenetics.Util;
using static Jenetics.Internal.Util.Require;

namespace Jenetics
{
    public class MonteCarloSelector<TGene, TAllele> : ISelector<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        public Population<TGene, TAllele> Select(Population<TGene, TAllele> population, int count, Optimize opt)
        {
            NonNull(population, "Population");
            if (count < 0)
                throw new ArgumentOutOfRangeException(
                    $"Selection count must be greater or equal then zero, but was {count}");

            var selection = new Population<TGene, TAllele>(count);
            if (count > 0 && !population.IsEmpty)
            {
                var random = RandomRegistry.GetRandom();
                var size = population.Count;
                for (var i = 0; i < count; ++i)
                {
                    var pos = random.NextInt(size);
                    selection.Add(population[pos]);
                }
            }

            return selection;
        }

        public override bool Equals(object obj)
        {
            return Equality.OfType(this, obj);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).Value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}";
        }
    }
}