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
using Jenetics.Util;

namespace Jenetics.Internal.Collection
{
    public class ArrayMutableSeq<T> : ArraySeqBase<T>, IMutableSeq<T>
    {
        public ArrayMutableSeq(Array<T> array) : base(array)
        {
        }

        T IMutableSeq<T>.this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }

        public IImmutableSeq<T> ToImmutableSeq()
        {
            return IsEmpty ? Empty.ImmutableSeq<T>() : new ArrayImmutableSeq<T>(Values.Seal());
        }

        ISeq<T> ISeq<T>.SubSeq(int start)
        {
            return SubSeq(start);
        }

        ISeq<T> ISeq<T>.SubSeq(int start, int end)
        {
            return SubSeq(start, end);
        }

        public new IMutableSeq<T> SubSeq(int start, int end)
        {
            if (start > end)
                throw new IndexOutOfRangeException($"start[{start}] > end[{end}]");
            if (start < 0 || end > Length)
                throw new IndexOutOfRangeException($"Indexes ({start}, {end}) range: [{0}..{Length})");

            return start == end
                ? Empty.MutableSeq<T>()
                : new ArrayMutableSeq<T>(Values.Slice(start, end));
        }

        public new IMutableSeq<T> SubSeq(int start)
        {
            if (start < 0 || start > Length)
                throw new IndexOutOfRangeException($"Index {start} range: [{0}..{Length})");

            return start == Length
                ? Empty.MutableSeq<T>()
                : new ArrayMutableSeq<T>(Values.Slice(start, Length));
        }

        public IMutableSeq<T> Copy()
        {
            return IsEmpty ? this : new ArrayMutableSeq<T>(Values.Copy());
        }
    }
}