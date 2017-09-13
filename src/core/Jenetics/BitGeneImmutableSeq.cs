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
using Jenetics.Internal.Collection;
using Jenetics.Util;
using Array = System.Array;

namespace Jenetics
{
    [Serializable]
    public class BitGeneImmutableSeq : ArrayImmutableSeq<BitGene>, ICopyable<BitGeneMutableSeq>
    {
        internal BitGeneImmutableSeq(Array<BitGene> array) : base(array)
        {
        }

        public new BitGeneMutableSeq Copy()
        {
            return new BitGeneMutableSeq(Values.Copy());
        }

        internal void CopyTo(byte[] array)
        {
            var store = (BitGeneStore) Values.Store();
            Array.Copy(store.Array, 0, array, 0, store.Array.Length);
        }

        internal BitGeneImmutableSeq Of(byte[] genes, int length)
        {
            return new BitGeneImmutableSeq(Internal.Collection.Array.Of(BitGeneStore.Of(genes, length)).Seal());
        }
    }
}