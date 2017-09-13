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
    [Serializable]
    public class CharacterGene : IGene<char, CharacterGene>, IComparable<CharacterGene>
    {
        public static readonly CharSeq DefaultCharacters = new CharSeq(
            "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            " !\"$%&/()=?`{[]}\\+~*#';.:,-_<>|@^'"
        );

        private readonly bool _valid;
        private readonly CharSeq _validCharacters;

        private CharacterGene(CharSeq chars, int index)
        {
            Allele = chars[index];
            _validCharacters = chars;
            _valid = true;
        }

        private CharacterGene(char character, CharSeq validChars)
        {
            Allele = NonNull(character);
            _validCharacters = NonNull(validChars);
            _valid = _validCharacters.Contains(Allele);
        }

        public int CompareTo(CharacterGene that)
        {
            return Allele.CompareTo(that.Allele);
        }

        public bool IsValid => _valid;

        public CharacterGene NewInstance()
        {
            return Of(_validCharacters);
        }

        public CharacterGene NewInstance(char character)
        {
            return Of(character, _validCharacters);
        }

        public char Allele { get; }

        public bool IsValidCharacter(char character)
        {
            return _validCharacters.Contains(character);
        }

        public static CharacterGene Of(CharSeq validCharacters)
        {
            return new CharacterGene(
                validCharacters,
                RandomRegistry.GetRandom().Next(validCharacters.Length)
            );
        }

        public static CharacterGene Of(
            char character,
            CharSeq validCharacters
        )
        {
            return new CharacterGene(character, validCharacters);
        }

        public static CharacterGene Of()
        {
            return new CharacterGene(
                DefaultCharacters,
                RandomRegistry.GetRandom().NextInt(DefaultCharacters.Length)
            );
        }

        public static CharacterGene Of(char character)
        {
            return new CharacterGene(character, DefaultCharacters);
        }

        public CharSeq GetValidCharacters()
        {
            return _validCharacters;
        }

        public override bool Equals(object obj)
        {
            return obj is CharacterGene gene &&
                   Equality.Eq(gene.Allele, Allele) &&
                   Equality.Eq(gene._validCharacters, _validCharacters);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType())
                .And(Allele)
                .And(_validCharacters).Value;
        }

        public override string ToString()
        {
            return Allele.ToString();
        }

        internal static IImmutableSeq<CharacterGene> Seq(CharSeq chars, int length)
        {
            var r = RandomRegistry.GetRandom();

            return MutableSeq.OfLength<CharacterGene>(length)
                .Fill(() => new CharacterGene(chars, r.Next(chars.Length)))
                .ToImmutableSeq();
        }
    }
}