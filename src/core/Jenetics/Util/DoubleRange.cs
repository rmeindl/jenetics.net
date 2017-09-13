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
    public class DoubleRange : IEnumerable<double>
    {
        private DoubleRange(double min, double max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException($"Min greater than max: {min} > {max}");

            Min = min;
            Max = max;
        }

        public double Min { get; }

        public double Max { get; }

        public IEnumerator<double> GetEnumerator()
        {
            var current = Min;
            while (current < Max)
            {
                yield return current;
                current = current + 1;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static DoubleRange Of(double min, double max)
        {
            return new DoubleRange(min, max);
        }

        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType())
                return false;
            return Min.CompareTo(((DoubleRange) other).Min) == 0 && Max.CompareTo(((DoubleRange) other).Max) == 0;
        }

        public override int GetHashCode()
        {
            return (int) (Min + 31 * Max);
        }

        public override string ToString()
        {
            return $"[{Min}, {Max}]";
        }
    }
}