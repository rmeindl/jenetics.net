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

namespace Jenetics.Util
{
    public interface ISeq<out T> : IEnumerable<T>
    {
        T this[int index] { get; }

        int Length { get; }

        bool IsEmpty { get; }

        int IndexWhere(Func<T, bool> predicate, int start, int end);
        
        ISeq<T> SubSeq(int start);
        
        ISeq<T> SubSeq(int start, int end);
    }

    public static class Seq
    {
        public static int GetHashCode<T>(ISeq<T> seq)
        {
            var hash = 1;
            foreach (object element in seq)
                hash = 31 * hash + (element == null ? 0 : element.GetHashCode());
            return hash;
        }

        public static bool Equals<T>(ISeq<T> seq, object obj)
        {
            if (ReferenceEquals(obj, seq))
                return true;
            if (!(obj is ISeq<T>))
                return false;

            var other = (ISeq<T>) obj;
            var equals = seq.Length == other.Length;
            for (var i = seq.Length; equals && --i >= 0;)
            {
                object element = seq[i];
                if (element != null)
                    equals = element.Equals(other[i]);
                else
                    equals = other[i] == null;
            }
            return equals;
        }

        public static bool ForAll<T>(this ISeq<T> seq, Func<T, bool> predicate)
        {
            var valid = true;
            for (int i = 0, n = seq.Length; i < n && valid; ++i)
                valid = predicate(seq[i]);
            return valid;
        }

        public static string ToString<T>(this ISeq<T> seq, string prefix, string separator, string suffix)
        {
            return prefix + string.Join(separator, seq) + suffix;
        }

        public static int IndexOf<T>(this ISeq<T> seq, object element)
        {
            return seq.IndexOf(element, 0, seq.Length);
        }

        public static int IndexOf<T>(this ISeq<T> seq, object element, int start, int end)
        {
            return element != null
                ? seq.IndexWhere(e => element.Equals(e), start, end)
                : seq.IndexWhere(e => e == null, start, end);
        }

        public static T[] ToArray<T>(this ISeq<T> seq, T[] array)
        {
            if (array.Length < seq.Length)
            {
                var copy = new T[seq.Length];
                for (var i = seq.Length; --i >= 0;)
                    copy[i] = seq[i];

                return copy;
            }

            for (int i = 0, n = seq.Length; i < n; ++i)
                array[i] = seq[i];
            if (array.Length > seq.Length)
                array[seq.Length] = default;

            return array;
        }
        
        public static bool Contains<T>(this ISeq<T> seq, object element) {
            return IndexOf(seq, element) != -1;
        }

        public static int IndexWhere<T>(this ISeq<T> seq, Func<T, bool> predicate)
        {
            return seq.IndexWhere(predicate, 0, seq.Length);
        }
    }
}