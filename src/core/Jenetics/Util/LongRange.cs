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

namespace Jenetics.Util
{
    public class LongRange : IEnumerable<long>
    {
        private LongRange(long min, long max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException($"Min greater than max: {min} > {max}");

            Min = min;
            Max = max;
        }

        public long Min { get; }

        public long Max { get; }

        public static LongRange Of(long min, long max)
        {
            return new LongRange(min, max);
        }

        public IEnumerator<long> GetEnumerator()
        {
            var current = Min;
            while (current < Max)
            {
                yield return current;
                current = current + 1;
            }
        }

        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType())
                return false;
            return Min == ((LongRange) other).Min && Max == ((LongRange) other).Max;
        }

        public override int GetHashCode()
        {
            return (int) (Min + 31 * Max);
        }

        public override string ToString()
        {
            return $"[{Min}, {Max}]";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}