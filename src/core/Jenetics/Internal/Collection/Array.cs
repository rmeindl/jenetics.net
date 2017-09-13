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

namespace Jenetics.Internal.Collection
{
    public static class Array
    {
        public static Array<T> Of<T>(IStore<T> store)
        {
            return new Array<T>(store);
        }

        public static Array<T> OfLength<T>(int length)
        {
            return new Array<T>(ObjectStore.OfLength<T>(length));
        }
    }

    [Serializable]
    public class Array<T> : IEnumerable<T>
    {
        private readonly int _start;
        private readonly StoreRef<T> _store;

        private Array(StoreRef<T> store, int from, int until)
        {
            _store = store;
            _start = from;
            Length = until - from;
        }

        public Array(IStore<T> store) : this(StoreRef.Of(store), 0, store.Length)
        {
        }

        public int Length { get; }

        public T this[int index]
        {
            get => _store[index + _start];
            set => _store[index + _start] = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = _start; i < Length; i++)
                yield return _store[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Array<T> Seal()
        {
            return new Array<T>(_store.Seal(), _start, Length + _start);
        }

        public IStore<T> Store()
        {
            return _store.Value;
        }

        public Array<T> Copy()
        {
            return new Array<T>(_store.Copy(_start, Length + _start));
        }

        public Array<T> Slice(int from, int until)
        {
            CheckIndex(from, until);
            return new Array<T>(_store, from + _start, until + _start);
        }

        public void CheckIndex(int from, int until)
        {
            if (from > until)
                throw new IndexOutOfRangeException($"fromIndex({from}) > toIndex({until})");
            if (from < 0 || until > Length)
                throw new IndexOutOfRangeException($"Invalid index range: [{from}, {until})");
        }
    }
}