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
using System.Collections;
using System.Collections.Generic;
using Jenetics.Util;
using static Jenetics.Internal.Util.Require;

namespace Jenetics.Internal.Collection
{
    [Serializable]
    public abstract class ArraySeqBase<T> : ISeq<T>
    {
        protected Array<T> Values;

        protected ArraySeqBase(Array<T> array)
        {
            Values = NonNull(array, "Array must not be null.");
        }

        public T this[int index] => Values[index];

        public int Length => Values.Length;

        public bool IsEmpty => Values.Length == 0;

        public int IndexWhere(Func<T, bool> predicate, int start, int end)
        {
            Values.CheckIndex(start, end);

            var index = -1;

            for (var i = start; i < end && index == -1; ++i)
                if (predicate(Values[i]))
                    index = i;

            return index;
        }

        public virtual ISeq<T> SubSeq(int start)
        {
            throw new NotSupportedException();
        }

        public virtual ISeq<T> SubSeq(int start, int end)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
                return true;
            if (!(obj is ISeq<T>))
                return false;

            var seq = (ISeq<T>) obj;
            return Seq.Equals(this, seq);
        }

        public override int GetHashCode()
        {
            return Seq.GetHashCode(this);
        }

        public override string ToString()
        {
            return this.ToString("[", ",", "]");
        }
    }
}