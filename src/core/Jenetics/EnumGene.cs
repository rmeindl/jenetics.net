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

namespace Jenetics
{
    [Serializable]
    public class EnumGene<TAllele> : IGene<TAllele, EnumGene<TAllele>>, IComparable<EnumGene<TAllele>>
    {
        private readonly int _alleleIndex;
        private readonly IImmutableSeq<TAllele> _validAlleles;

        public EnumGene(int alleleIndex, IImmutableSeq<TAllele> validAlleles)
        {
            if (validAlleles.IsEmpty)
                throw new ArgumentException(
                    "Array of valid alleles must be greater than zero."
                );

            if (alleleIndex < 0 || alleleIndex >= validAlleles.Length)
                throw new IndexOutOfRangeException($"Allele index is not in range [0, {alleleIndex}).");

            _validAlleles = validAlleles;
            _alleleIndex = alleleIndex;
        }

        public int CompareTo(EnumGene<TAllele> gene)
        {
            var result = 0;
            if (_alleleIndex > gene._alleleIndex)
                result = 1;
            else if (_alleleIndex < gene._alleleIndex)
                result = -1;

            return result;
        }

        public TAllele Allele => _validAlleles[_alleleIndex];

        public bool IsValid => _alleleIndex >= 0 && _alleleIndex < _validAlleles.Length;

        public EnumGene<TAllele> NewInstance()
        {
            return new EnumGene<TAllele>(
                RandomRegistry.GetRandom().NextInt(_validAlleles.Length),
                _validAlleles
            );
        }

        public EnumGene<TAllele> NewInstance(TAllele value)
        {
            return new EnumGene<TAllele>(
                _validAlleles.IndexOf(value),
                _validAlleles
            );
        }

        public IImmutableSeq<TAllele> GetValidAlleles()
        {
            return _validAlleles;
        }

        public int GetAlleleIndex()
        {
            return _alleleIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is EnumGene<TAllele> gene &&
                   Equality.Eq(gene._alleleIndex, _alleleIndex) &&
                   Equality.Eq(gene._validAlleles, _validAlleles);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType())
                .And(_alleleIndex)
                .And(_validAlleles).Value;
        }

        public override string ToString()
        {
            return $"{Allele}";
        }
    }

    public static class EnumGene
    {
        public static EnumGene<TAllele> Of<TAllele>(
            int alleleIndex,
            IImmutableSeq<TAllele> validAlleles
        )
        {
            return new EnumGene<TAllele>(alleleIndex, validAlleles);
        }

        public static EnumGene<TAllele> Of<TAllele>(IImmutableSeq<TAllele> validAlleles)
        {
            return new EnumGene<TAllele>(
                RandomRegistry.GetRandom().NextInt(validAlleles.Length),
                validAlleles
            );
        }
    }
}