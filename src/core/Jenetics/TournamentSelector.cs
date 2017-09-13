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
using System.Collections.Generic;
using System.Linq;
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics
{
    public class TournamentSelector<TGene, TAllele> : ISelector<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly int _sampleSize;

        public TournamentSelector(int sampleSize)
        {
            if (sampleSize < 2)
                throw new ArgumentOutOfRangeException($"Sample size must be greater than one, but was {sampleSize}");
            _sampleSize = sampleSize;
        }

        public Population<TGene, TAllele> Select(Population<TGene, TAllele> population, int count, Optimize opt)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(
                    $"Selection count must be greater or equal then zero, but was {count}");
            var random = RandomRegistry.GetRandom();
            return population.IsEmpty
                ? new Population<TGene, TAllele>(0)
                : new Population<TGene, TAllele>(count)
                    .Fill(() => Select(population, opt, _sampleSize, random), count);
        }

        private Phenotype<TGene, TAllele> Select(Population<TGene, TAllele> population, Optimize opt, int sampleSize,
            Random random)
        {
            return SelectAtRandom(population, random).Take(sampleSize)
                .OrderBy(p => p, opt.Descending<Phenotype<TGene, TAllele>>()).FirstOrDefault();
        }

        private static IEnumerable<Phenotype<TGene, TAllele>> SelectAtRandom(Population<TGene, TAllele> population,
            Random random)
        {
            var n = population.Count;

            foreach (var t in population)
                yield return population[random.NextInt(n)];
        }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(s => _sampleSize == s._sampleSize);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(_sampleSize).Value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}[s={_sampleSize}]";
        }
    }
}