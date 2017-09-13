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
    public class CompositeAlterer<TGene, TAllele> : AltererBase<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        public CompositeAlterer(ISeq<IAlterer<TGene, TAllele>> alterers) : base(1.0)
        {
            Alterers = Normalize(alterers);
        }

        public IImmutableSeq<IAlterer<TGene, TAllele>> Alterers { get; }

        private static IImmutableSeq<IAlterer<TGene, TAllele>> Normalize(ISeq<IAlterer<TGene, TAllele>> alterers)
        {
            IEnumerable<IAlterer<TGene, TAllele>> Mapper(IAlterer<TGene, TAllele> a)
            {
                return a is CompositeAlterer<TGene, TAllele> alterer
                    ? alterer.Alterers
                    : Enumerable.Repeat(a, 1);
            }

            return alterers.SelectMany(Mapper).ToList().ToImmutableSeq();
        }

        public override int Alter(Population<TGene, TAllele> population, long generation)
        {
            return Alterers.Select(a => a.Alter(population, generation)).Sum();
        }

        public override bool Equals(object obj)
        {
            return obj is CompositeAlterer<TGene, TAllele> alterer &&
                   Equality.Eq(alterer.Alterers, Alterers);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(Alterers).Value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}:\n{string.Join("\n", Alterers.Select(a => "   - " + a))}";
        }
    }

    public static class CompositeAlterer
    {
        public static CompositeAlterer<TGene, TAllele> Of<TGene, TAllele>(params IAlterer<TGene, TAllele>[] alterers)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return new CompositeAlterer<TGene, TAllele>(ImmutableSeq.Of(alterers));
        }

        public static CompositeAlterer<TGene, TAllele> Join<TGene, TAllele>(
            IAlterer<TGene, TAllele> a1,
            IAlterer<TGene, TAllele> a2
        )
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return Of(a1, a2);
        }
    }
}