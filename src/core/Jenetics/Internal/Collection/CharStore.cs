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
    [Serializable]
    public class CharStore : IStore<char>
    {
        private readonly char[] _array;

        private CharStore(char[] chars)
        {
            _array = chars;
        }

        public CharStore(int length) : this(new char[length])
        {
        }

        public int Length => _array.Length;

        public char this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        public void Sort(int from, int until, IComparer<char> comparator)
        {
            if (comparator == null)
            {
                System.Array.Sort(_array, from, until);
            }
            else
            {
                var chars = new char[_array.Length];
                for (var i = 0; i < _array.Length; ++i)
                    chars[i] = _array[i];
                System.Array.Sort(chars, from, until, comparator);
                for (var i = 0; i < _array.Length; ++i)
                    _array[i] = chars[i];
            }
        }

        public IStore<char> Copy(int from, int until)
        {
            var newArray = new char[until - from];
            System.Array.Copy(_array, from, newArray, 0, until - from);
            return new CharStore(newArray);
        }

        public IStore<char> NewInstance(int length)
        {
            return new CharStore(length);
        }

        public static CharStore Of(char[] chars)
        {
            return new CharStore(chars);
        }
    }
}