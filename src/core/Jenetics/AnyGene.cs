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
    public class AnyGene<TAllele> : IGene<TAllele, AnyGene<TAllele>>
    {
        private readonly Func<TAllele> _supplier;
        private readonly Func<TAllele, bool> _validator;

        internal AnyGene(
            TAllele allele,
            Func<TAllele> supplier,
            Func<TAllele, bool> validator
        )
        {
            Allele = allele;
            _supplier = NonNull(supplier);
            _validator = NonNull(validator);
        }

        public bool IsValid =>  _validator(Allele);

        public AnyGene<TAllele> NewInstance()
        {
            return new AnyGene<TAllele>(_supplier(), _supplier, _validator);
        }

        public TAllele Allele { get; }

        public AnyGene<TAllele> NewInstance(TAllele value)
        {
            return new AnyGene<TAllele>(value, _supplier, _validator);
        }

        public override bool Equals(object obj)
        {
            return obj is AnyGene<TAllele> gene &&
                   Equality.Eq(gene.Allele, Allele);
        }

        public override int GetHashCode()
        {
            return Allele.GetHashCode();
        }

        public override string ToString()
        {
            return Allele.ToString();
        }
    }

    public static class AnyGene
    {
        public static AnyGene<TAllele> Of<TAllele>(
            TAllele allele,
            Func<TAllele> supplier,
            Func<TAllele, bool> validator
        )
        {
            return new AnyGene<TAllele>(allele, supplier, validator);
        }

        public static AnyGene<TAllele> Of<TAllele>(Func<TAllele> supplier)
        {
            return new AnyGene<TAllele>(supplier(), supplier, a => true);
        }

        public static AnyGene<TAllele> Of<TAllele>(TAllele allele, Func<TAllele> supplier)
        {
            return new AnyGene<TAllele>(allele, supplier, a => true);
        }

        public static AnyGene<TAllele> Of<TAllele>(
            Func<TAllele> supplier,
            Func<TAllele, bool> validator
        )
        {
            return new AnyGene<TAllele>(supplier(), supplier, validator);
        }

        internal static IImmutableSeq<AnyGene<TAllele>> Seq<TAllele>(
            int length,
            Func<TAllele> supplier,
            Func<TAllele, bool> validator
        )
        {
            return MutableSeq.OfLength<AnyGene<TAllele>>(length)
                .Fill(() => Of(supplier(), supplier, validator))
                .ToImmutableSeq();
        }
    }
}