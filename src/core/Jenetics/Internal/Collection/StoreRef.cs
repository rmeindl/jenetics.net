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

namespace Jenetics.Internal.Collection
{
    public static class StoreRef
    {
        public static StoreRef<TSource> Of<TSource>(IStore<TSource> value)
        {
            return new StoreRef<TSource>(value, false);
        }
    }

    [Serializable]
    public class StoreRef<T> : IStore<T>
    {
        private bool _sealed;

        public StoreRef(IStore<T> value, bool isSealed)
        {
            Value = value;
            _sealed = isSealed;
        }

        internal IStore<T> Value { get; private set; }

        public int Length => Value.Length;

        public T this[int index]
        {
            get => Value[index];
            set
            {
                CopyIfSealed();
                Value[index] = value;
            }
        }

        public void Sort(int from, int until, IComparer<T> comparator)
        {
            CopyIfSealed();
            Value.Sort(from, until, comparator);
        }

        public IStore<T> Copy(int from, int until)
        {
            return Value.Copy(from, until);
        }

        public IStore<T> NewInstance(int length)
        {
            return Value.NewInstance(length);
        }

        public StoreRef<T> Seal()
        {
            _sealed = true;
            return new StoreRef<T>(Value, true);
        }

        public bool IsSealed()
        {
            return _sealed;
        }

        private void CopyIfSealed()
        {
            if (_sealed)
            {
                Value = this.Copy();
                _sealed = false;
            }
        }
    }
}