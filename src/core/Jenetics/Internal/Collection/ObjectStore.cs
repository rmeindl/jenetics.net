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
using Jenetics.Internal.Util;

namespace Jenetics.Internal.Collection
{
    public interface IStore<T>
    {
        int Length { get; }

        T this[int index] { get; set; }

        void Sort(int from, int until, IComparer<T> comparator);

        IStore<T> Copy(int from, int until);

        IStore<T> NewInstance(int length);
    }

    public static class ObjectStore
    {
        public static ObjectStore<TSource> OfLength<TSource>(int length)
        {
            return new ObjectStore<TSource>(new TSource[length]);
        }

        public static ObjectStore<T> Of<T>(T[] array)
        {
            return new ObjectStore<T>(array);
        }

        public static IStore<T> Copy<T>(this IStore<T> store)
        {
            return store.Copy(0, store.Length);
        }
    }

    [Serializable]
    public class ObjectStore<T> : IStore<T>
    {
        private readonly T[] _array;

        public ObjectStore(T[] array)
        {
            _array = Require.NonNull(array);
        }

        public int Length => _array.Length;

        public T this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        public void Sort(
            int from,
            int until,
            IComparer<T> comparator
        )
        {
            System.Array.Sort(_array, from, until, comparator);
        }

        public IStore<T> Copy(int from, int until)
        {
            var newArray = new T[until - from];
            System.Array.Copy(_array, from, newArray, 0, until - from);
            return new ObjectStore<T>(newArray);
        }

        public IStore<T> NewInstance(int length)
        {
            return OfLength(length);
        }

        public static ObjectStore<T> Of(T[] array)
        {
            return new ObjectStore<T>(array);
        }

        public static ObjectStore<T> OfLength(int length)
        {
            return new ObjectStore<T>(new T[length]);
        }
    }
}