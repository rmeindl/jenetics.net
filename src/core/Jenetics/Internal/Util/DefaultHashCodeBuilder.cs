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
using Jenetics.Util;

namespace Jenetics.Internal.Util
{
    internal class DefaultHashCodeBuilder : IHash
    {
        private const int P1 = 47;
        private const int P2 = 103;

        private int _hash;

        internal DefaultHashCodeBuilder(Type type)
        {
            _hash = type.GetHashCode();
        }

        public IHash And(byte[] values)
        {
            return AndEnumerable(values);
        }

        public IHash And(char[] values)
        {
            return AndEnumerable(values);
        }


        public IHash And<T>(ISeq<T> values)
        {
            _hash += values.GetHashCode();
            return this;
        }

        public IHash And<T>(IImmutableSeq<T> values)
        {
            _hash += values.GetHashCode();
            return this;
        }

        public IHash And(char value)
        {
            _hash += P1 * value + P2;
            return this;
        }

        public IHash And(double value)
        {
            _hash += P1 * value.GetHashCode();
            return this;
        }

        public IHash And(int value)
        {
            _hash += P1 * value + P2;
            return this;
        }

        public IHash And(long value)
        {
            _hash += P1 * value.GetHashCode();
            return this;
        }

        public IHash And<T>(T value)
        {
            _hash += P1 * (value?.GetHashCode() ?? 0) + P2;
            return this;
        }

        public IHash And<T>(List<T> values)
        {
            return AndEnumerable(values);
        }

        public int Value => _hash;

        private IHash AndEnumerable<T>(IEnumerable<T> values)
        {
            if (values == null) return this;

            const int hash = 17;
            foreach (var element in values)
                _hash += hash * 31 + element.GetHashCode();

            return this;
        }
    }
}