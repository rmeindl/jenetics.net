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

using Jenetics.Internal.Collection;
using Jenetics.Util;

namespace Jenetics
{
    public class BitGeneMutableSeq : ArrayMutableSeq<BitGene>, IToImmutableSeq<BitGeneImmutableSeq>
    {
        internal BitGeneMutableSeq(Array<BitGene> array) : base(array)
        {
        }

        public new BitGeneImmutableSeq ToImmutableSeq()
        {
            return new BitGeneImmutableSeq(Values.Copy().Seal());
        }

        internal static BitGeneMutableSeq Of(byte[] genes, int length)
        {
            return new BitGeneMutableSeq(Array.Of(BitGeneStore.Of(genes, length)));
        }

        public static BitGeneMutableSeq Of(Array<BitGene> array)
        {
            return new BitGeneMutableSeq(array);
        }
    }
}