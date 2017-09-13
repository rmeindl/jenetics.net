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

using Xunit;

namespace Jenetics
{
    public class CharacterGeneTest : GeneTesterBase<char, CharacterGene>
    {
        protected override Factory<CharacterGene> Factory()
        {
            return CharacterGene.Of;
        }

        [Fact]
        public void TestCharacterGene()
        {
            var gene = CharacterGene.Of();
            Assert.True(gene.IsValidCharacter(gene.Allele));
        }

        [Fact]
        public void TestCharacterGeneCharacter()
        {
            var gene = CharacterGene.Of('4');

            Assert.Equal('4', gene.Allele);
        }

        [Fact]
        public void TestCompareTo()
        {
            var g1 = CharacterGene.Of('1');
            var g2 = CharacterGene.Of('2');
            var g3 = CharacterGene.Of('3');

            Assert.True(g1.CompareTo(g2) < 0);
            Assert.True(g2.CompareTo(g3) < 0);
            Assert.True(g3.CompareTo(g2) > 0);
            Assert.True(g2.CompareTo(g2) == 0);
        }

        [Fact]
        public void TestGetCharacter()
        {
            var gene = CharacterGene.Of('6');

            Assert.Equal('6', gene.Allele);
        }

        [Fact]
        public void TestGetValidCharacters()
        {
            var cset = CharacterGene.DefaultCharacters;
            Assert.NotNull(cset);
            Assert.False(cset.IsEmpty);
        }

        [Fact]
        public void TestIsValidCharacter()
        {
            foreach (var c in CharacterGene.DefaultCharacters)
                Assert.True(CharacterGene.Of(c).IsValidCharacter(c));
        }
    }
}