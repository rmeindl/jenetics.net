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
using Jenetics.Util;
using static Jenetics.Internal.Util.Require;

namespace Jenetics
{
    public class AnyChromosome<TAllele> : ChromosomeBase<AnyGene<TAllele>>
    {
        private readonly Func<IImmutableSeq<TAllele>, bool> _alleleSeqValidator;
        private readonly Func<TAllele, bool> _alleleValidator;
        private readonly Func<TAllele> _supplier;

        private bool? _valid;

        protected internal AnyChromosome(
            IImmutableSeq<AnyGene<TAllele>> genes,
            Func<TAllele> supplier,
            Func<TAllele, bool> alleleValidator,
            Func<IImmutableSeq<TAllele>, bool> alleleSeqValidator
        ) : base(genes)
        {
            _supplier = NonNull(supplier);
            _alleleValidator = NonNull(alleleValidator);
            _alleleSeqValidator = NonNull(alleleSeqValidator);
        }

        public override IEnumerator<AnyGene<TAllele>> GetEnumerator()
        {
            return Genes.GetEnumerator();
        }

        public override IChromosome<AnyGene<TAllele>> NewInstance()
        {
            return AnyChromosome.Of(_supplier, _alleleValidator, _alleleSeqValidator, Length);
        }

        public override IChromosome<AnyGene<TAllele>> NewInstance(IImmutableSeq<AnyGene<TAllele>> genes)
        {
            return new AnyChromosome<TAllele>(
                genes,
                _supplier,
                _alleleValidator,
                _alleleSeqValidator
            );
        }

        public override bool IsValid
        {
            get
            {
                if (_valid == null)
                {
                    var alleles = ToSeq().Select(g => g.Allele).ToImmutableSeq();
                    _valid = _alleleSeqValidator(alleles) && alleles.All(_alleleValidator);
                }

                return (bool) _valid;
            }
        }
    }

    public static class AnyChromosome
    {
        public static AnyChromosome<TAllele> Of<TAllele>(
            Func<TAllele> supplier,
            int length = 1)
        {
            return Of(supplier, arg => true, length);
        }

        public static AnyChromosome<TAllele> Of<TAllele>(
            Func<TAllele> supplier,
            Func<TAllele, bool> validator,
            int length
        )
        {
            return Of(supplier, validator, arg => true, length);
        }

        public static AnyChromosome<TAllele> Of<TAllele>(
            Func<TAllele> supplier,
            Func<TAllele, bool> alleleValidator,
            Func<IImmutableSeq<TAllele>, bool> alleleSeqValidator,
            int length
        )
        {
            return new AnyChromosome<TAllele>(
                AnyGene.Seq(length, supplier, alleleValidator),
                supplier,
                alleleValidator,
                alleleSeqValidator
            );
        }
    }
}