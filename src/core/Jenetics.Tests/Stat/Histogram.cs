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
using System.Linq;
using Jenetics.Internal.Util;
using static Jenetics.Internal.Util.Require;

namespace Jenetics.Stat
{
    public class Histogram<T>
    {
        private readonly IComparer<T> _comparator;
        private readonly long[] _histogram;
        private readonly T[] _separators;

        private long _count;

        public Histogram(IComparer<T> comparator, params T[] separators)
        {
            _separators = Histogram.Check(separators);
            _comparator = NonNull(comparator, "Comparator");
            _histogram = new long[separators.Length + 1];

            Array.Sort(_separators, _comparator);
            Array.Fill(_histogram, 0L);
        }

        public int Length => _histogram.Length;

        public void Accept(T value)
        {
            ++_count;
            ++_histogram[Index(value)];
        }

        public double[] GetHistogram(double[] histogram)
        {
            NonNull(histogram);

            var hist = histogram;
            if (histogram.Length >= _histogram.Length)
                Array.Copy(_histogram, 0, hist, 0, _histogram.Length);
            else
                Array.Copy(_histogram, 0, hist, 0, _histogram.Length);

            return hist;
        }

        public double[] GetHistogram()
        {
            return GetHistogram(new double[_histogram.Length]);
        }

        public void Combine(Histogram<T> other)
        {
            if (!Equality.Eq(_separators, other._separators))
                throw new ArgumentException(
                    "The histogram separators are not equals."
                );

            _count += other._count;
            for (var i = other._histogram.Length; --i >= 0;)
                _histogram[i] += other._histogram[i];
        }

        private int Index(T value)
        {
            var low = 0;
            var high = _separators.Length - 1;

            while (low <= high)
            {
                if (_comparator.Compare(value, _separators[low]) < 0)
                    return low;
                if (_comparator.Compare(value, _separators[high]) >= 0)
                    return high + 1;

                var mid = Bits.bit_rol(low + high, 1);
                if (_comparator.Compare(value, _separators[mid]) < 0)
                    high = mid;
                else if (_comparator.Compare(value, _separators[mid]) >= 0)
                    low = mid + 1;
            }

            throw new Exception("This line will never be reached.");
        }
    }

    public static class Histogram
    {
        public static Histogram<T> Of<T>(
            params T[] separators
        )
            where T : IComparable<T>
        {
            return new Histogram<T>(Comparer<T>.Create((o1, o2) => o1.CompareTo(o2)), separators);
        }

        public static Histogram<double> OfDouble(
            double min,
            double max,
            int nclasses
        )
        {
            return Of(ToSeparators(min, max, nclasses));
        }

        public static Histogram<long> OfLong(long min, long max, int nclasses)
        {
            return Of(ToSeparators(min, max, nclasses));
        }

        public static Histogram<T> ToDoubleHistogram<T>(this IEnumerable<Histogram<T>> values, double min, double max,
            int classCount)
        {
            return values.Aggregate((workingSentence, next) =>
            {
                workingSentence.Combine(next);
                return workingSentence;
            });
        }

        private static double[] ToSeparators(
            double min,
            double max,
            int nclasses
        )
        {
            Check(min, max, nclasses);

            var stride = (max - min) / nclasses;
            var separators = new double[nclasses - 1];
            for (var i = 0; i < separators.Length; ++i)
                separators[i] = min + stride * (i + 1);

            return separators;
        }

        private static long[] ToSeparators(long min, long max, int nclasses)
        {
            Check(min, max, nclasses);

            var size = (int) (max - min);
            var pts = Math.Min(size, nclasses);
            var bulk = size / pts;
            var rest = size % pts;

            var separators = new long[pts - 1];
            for (int i = 1, n = pts - rest; i < n; ++i)
                separators[i - 1] = i * bulk + min;
            for (var i = 0; i < rest; ++i)
                separators[separators.Length - rest + i] =
                    (pts - rest) * bulk + i * (bulk + 1) + min;

            return separators;
        }

        internal static T[] Check<T>(params T[] classes)
        {
            foreach (var c in classes)
                NonNull(c);
            if (classes.Length == 0)
                throw new ArgumentException("Given classes array is empty.");

            return classes;
        }
    }
}