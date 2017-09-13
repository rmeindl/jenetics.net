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
using Jenetics.Internal.Collection;
using Jenetics.Internal.Util;

namespace Jenetics
{
    [Serializable]
    public class BitGeneStore : IStore<BitGene>
    {
        internal readonly byte[] Array;

        private BitGeneStore(byte[] array, int length)
        {
            Array = array;
            Length = length;
        }

        public int Length { get; }

        public BitGene this[int index]
        {
            get => BitGene.Of(Bits.Get(Array, index));
            set => Bits.Set(Array, index, value.BooleanValue());
        }

        public void Sort(int from, int until, IComparer<BitGene> comparator)
        {
            throw new NotSupportedException();
        }

        public IStore<BitGene> Copy(int from, int until)
        {
            return new BitGeneStore(Bits.Copy(Array, from, until), until - from);
        }

        public IStore<BitGene> NewInstance(int length)
        {
            return OfLength(length);
        }

        internal static BitGeneStore Of(byte[] array, int length)
        {
            return new BitGeneStore(array, length);
        }

        public static BitGeneStore OfLength(int length)
        {
            return new BitGeneStore(Bits.NewArray(length), length);
        }
    }
}