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
using System.Linq;
using Jenetics.Util;

namespace Jenetics.Internal.Collection
{
    public static class Empty
    {
        public static IMutableSeq<T> MutableSeq<T>()
        {
            return new DefaultMutableSeq<T>();
        }

        public static IImmutableSeq<T> ImmutableSeq<T>()
        {
            return new DefaultImmutableSeq<T>();
        }

        private class DefaultMutableSeq<T> : IMutableSeq<T>
        {
            public IEnumerator<T> GetEnumerator()
            {
                return Enumerable.Empty<T>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public T this[int index]
            {
                get => throw new IndexOutOfRangeException("MutableSeq is empty.");
                set => throw new IndexOutOfRangeException("MutableSeq is empty.");
            }

            public IImmutableSeq<T> ToImmutableSeq()
            {
                return ImmutableSeq<T>();
            }

            ISeq<T> ISeq<T>.SubSeq(int start)
            {
                return SubSeq(start);
            }

            ISeq<T> ISeq<T>.SubSeq(int start, int end)
            {
                return SubSeq(start, end);
            }

            public IMutableSeq<T> SubSeq(int start, int end)
            {
                throw new IndexOutOfRangeException("MutableSeq is empty.");
            }

            public IMutableSeq<T> SubSeq(int start)
            {
                throw new IndexOutOfRangeException("MutableSeq is empty.");
            }

            public int Length => 0;

            public bool IsEmpty => true;

            public int IndexWhere(Func<T, bool> predicate, int start, int end)
            {
                return -1;
            }

            public IMutableSeq<T> Copy()
            {
                return this;
            }
        }

        private class DefaultImmutableSeq<T> : IImmutableSeq<T>
        {
            public IImmutableSeq<TR> Map<TR>(Func<T, TR> mapper)
            {
                return ImmutableSeq<TR>();
            }

            ISeq<T> ISeq<T>.SubSeq(int start)
            {
                return SubSeq(start);
            }

            ISeq<T> ISeq<T>.SubSeq(int start, int end)
            {
                return SubSeq(start, end);
            }

            public IImmutableSeq<T> SubSeq(int start, int end)
            {
                throw new IndexOutOfRangeException("ImmutableSeq is empty.");
            }

            public IImmutableSeq<T> SubSeq(int start)
            {
                throw new IndexOutOfRangeException("ImmutableSeq is empty.");
            }

            public IEnumerator<T> GetEnumerator()
            {
                return Enumerable.Empty<T>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public T this[int index] => throw new IndexOutOfRangeException("ImmutableSeq is empty.");

            public int Length => 0;

            public bool IsEmpty => true;

            public int IndexWhere(Func<T, bool> predicate, int start, int end)
            {
                return -1;
            }

            public IMutableSeq<T> Copy()
            {
                return Jenetics.Util.MutableSeq.Empty<T>();
            }
        }
    }
}