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
using Jenetics.Internal.Math;
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics
{
    [Serializable]
    public class PermutationChromosome<T> : ChromosomeBase<EnumGene<T>>
    {
        private readonly IImmutableSeq<T> _validAlleles;

        public PermutationChromosome(IImmutableSeq<EnumGene<T>> genes, bool? valid = null) : base(genes)
        {
            _validAlleles = genes[0].GetValidAlleles();
            Valid = valid;
        }

        public override IEnumerator<EnumGene<T>> GetEnumerator()
        {
            return Genes.GetEnumerator();
        }

        public override IChromosome<EnumGene<T>> NewInstance()
        {
            return PermutationChromosome.Of(_validAlleles, Length);
        }

        public override IChromosome<EnumGene<T>> NewInstance(IImmutableSeq<EnumGene<T>> genes)
        {
            return new PermutationChromosome<T>(genes);
        }

        public IImmutableSeq<T> GetValidAlleles()
        {
            return _validAlleles;
        }

        public override bool IsValid
        {
            get
            {
                if (Valid == null)
                {
                    var check = Bits.NewArray(_validAlleles.Length);
                    Valid = Genes.All(g => !Bits.GetAndSet(check, g.GetAlleleIndex()));
                }

                return (bool) Valid;
            }
        }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(e => base.Equals(e));
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(base.GetHashCode()).Value;
        }

        public override string ToString()
        {
            return string.Join("|", Genes.Select(g => g.Allele.ToString()));
        }
    }

    public static class PermutationChromosome
    {
        public static PermutationChromosome<T> Of<T>(
            IImmutableSeq<T> alleles,
            int length
        )
        {
            Require.Positive(length);
            if (length > alleles.Length)
                throw new ArgumentException(
                    $"The sub-set size must be be greater then the base-set: {length} > {alleles.Length}");

            var subset = Arrays.Shuffle(Base.Subset(alleles.Length, length));
            var genes = subset.Select(i => EnumGene.Of(i, alleles)).ToImmutableSeq();

            return new PermutationChromosome<T>(genes, true);
        }

        public static PermutationChromosome<T> Of<T>(IImmutableSeq<T> alleles)
        {
            return Of(alleles, alleles.Length);
        }

        public static PermutationChromosome<int> OfInteger(int length)
        {
            return OfInteger(0, Require.Positive(length));
        }

        public static PermutationChromosome<int>
            OfInteger(int start, int end)
        {
            if (end <= start)
                throw new ArgumentException($"end <= start: {end} <= {start}");

            return OfInteger(IntRange.Of(start, end), end - start);
        }

        public static PermutationChromosome<int>
            OfInteger(IntRange range, int length)
        {
            return Of(
                range.Select(i => i).ToImmutableSeq(),
                length
            );
        }
    }
}