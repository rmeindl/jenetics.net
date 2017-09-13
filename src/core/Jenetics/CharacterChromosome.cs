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
using System.Runtime.Serialization;
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics
{
    [Serializable]
    public class CharacterChromosome : ChromosomeBase<CharacterGene>, ISerializable
    {
        private readonly CharSeq _validCharacters;

        public CharacterChromosome(IImmutableSeq<CharacterGene> genes) : base(genes)
        {
            _validCharacters = genes[0].GetValidCharacters();
        }

        public CharacterChromosome(CharSeq validCharacters, int length) : this(CharacterGene.Seq(validCharacters,
            length))
        {
            Valid = true;
        }

        protected CharacterChromosome(SerializationInfo info, StreamingContext context)
        {
            var length = info.GetInt32("_length");
            _validCharacters = (CharSeq) info.GetValue("_validCharacters", typeof(CharSeq));

            var genes = MutableSeq.OfLength<CharacterGene>(length);
            for (var i = 0; i < length; ++i)
            {
                var gene = CharacterGene.Of(
                    info.GetChar("_gene_" + i),
                    _validCharacters
                );
                genes[i] = gene;
            }

            Genes = genes.ToImmutableSeq();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_length", Length);
            info.AddValue("_validCharacters", _validCharacters);

            for (var i = 0; i < Genes.Length; i++)
                info.AddValue("_gene_" + i, Genes[i].Allele);
        }

        public override IEnumerator<CharacterGene> GetEnumerator()
        {
            return Genes.GetEnumerator();
        }

        public override IChromosome<CharacterGene> NewInstance()
        {
            return new CharacterChromosome(_validCharacters, Length);
        }

        public override IChromosome<CharacterGene> NewInstance(IImmutableSeq<CharacterGene> genes)
        {
            return new CharacterChromosome(genes);
        }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(cc =>
                base.Equals(obj) &&
                Equality.Eq(_validCharacters, cc._validCharacters)
            );
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType())
                .And(base.GetHashCode())
                .And(_validCharacters).Value;
        }

        public override string ToString()
        {
            return string.Join("", ToSeq());
        }

        public static CharacterChromosome Of(int length)
        {
            return new CharacterChromosome(
                CharacterGene.Seq(CharacterGene.DefaultCharacters, length)
            );
        }

        public static CharacterChromosome Of(string alleles, CharSeq validChars)
        {
            var index = new IntRef();

            CharacterGene GeneFactory()
            {
                return CharacterGene.Of(alleles[index.Value++], validChars);
            }

            var genes = MutableSeq.OfLength<CharacterGene>(alleles.Length).Fill(GeneFactory).ToImmutableSeq();

            return new CharacterChromosome(genes);
        }

        public static CharacterChromosome Of(string alleles)
        {
            return Of(alleles, CharacterGene.DefaultCharacters);
        }
    }
}