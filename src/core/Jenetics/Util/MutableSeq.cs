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
using Jenetics.Internal.Collection;
using Jenetics.Internal.Util;
using Array = Jenetics.Internal.Collection.Array;

namespace Jenetics.Util
{
    public interface IToImmutableSeq<out T>
    {
        T ToImmutableSeq();
    }

    public interface IMutableSeq<T> : ISeq<T>, ICopyable<IMutableSeq<T>>, IToImmutableSeq<IImmutableSeq<T>>
    {
        new T this[int index] { get; set; }

        new IMutableSeq<T> SubSeq(int start, int end);
        new IMutableSeq<T> SubSeq(int start);
    }

    public static class MutableSeq
    {
        public static void Swap<T>(this IMutableSeq<T> seq, int i, int j)
        {
            var temp = seq[i];
            seq[i] = seq[j];
            seq[j] = temp;
        }

        public static IMutableSeq<T> OfLength<T>(int length)
        {
            return length == 0
                ? Empty<T>()
                : new ArrayMutableSeq<T>(Array.Of(ObjectStore.OfLength<T>(length)));
        }

        public static IMutableSeq<T> Empty<T>()
        {
            return Internal.Collection.Empty.MutableSeq<T>();
        }

        public static IMutableSeq<T> Of<T>(params T[] values)
        {
            return values.Length == 0
                ? Empty<T>()
                : new ArrayMutableSeq<T>(Array.Of(ObjectStore.Of((T[]) values.Clone())));
        }

        public static IMutableSeq<T> Of<T>(IEnumerable<T> values)
        {
            IMutableSeq<T> mseq;
            if (values is IImmutableSeq<T> immutableSeq)
            {
                var seq = immutableSeq;
                mseq = seq.IsEmpty ? Empty<T>() : Of<T>(seq);
            }
            else if (values is IMutableSeq<T>)
            {
                var seq = (IMutableSeq<T>) values;
                mseq = seq.IsEmpty ? Empty<T>() : Of<T>(seq);
            }
            else if (values is ICollection<T>)
            {
                var collection = (ICollection<T>) values;
                mseq = collection.Count == 0
                    ? Empty<T>()
                    : OfLength<T>(collection.Count).SetAll(values);
            }
            else
            {
                var array = values.ToArray();
                mseq = array.Length == 0 ? Empty<T>() : new ArrayMutableSeq<T>(Array.Of(ObjectStore.Of(array)));
            }

            return mseq;
        }

        public static IMutableSeq<T> Of<T>(ISeq<T> values)
        {
            return values is ArrayMutableSeq<T> seq ? seq.Copy() : OfLength<T>(values.Count()).SetAll(values);
        }

        public static IMutableSeq<T> Of<T>(Func<T> supplier, int length)
        {
            Require.NonNull(supplier);

            return length == 0
                ? Empty<T>()
                : OfLength<T>(length).Fill(supplier);
        }
    }

    public static class MutableSeqExtensions
    {
        public static IMutableSeq<T> Fill<T>(this IMutableSeq<T> seq, Func<T> supplier)
        {
            for (int i = 0, n = seq.Length; i < n; ++i)
                seq[i] = supplier();
            return seq;
        }

        public static IMutableSeq<T> SetAll<T>(this IMutableSeq<T> seq, IEnumerable<T> values)
        {
            var i = 0;
            foreach (var value in values)
                seq[i++] = value;
            return seq;
        }

        public static void Swap<T>(this IMutableSeq<T> seq, int start, int end, IMutableSeq<T> other, int otherStart)
        {
            if (otherStart < 0 || otherStart + (end - start) > seq.Length)
                throw new IndexOutOfRangeException(
                    $"Invalid index range: [{otherStart}, {otherStart + (end - start)})");

            if (start < end)
                for (var i = end - start; --i >= 0;)
                {
                    var temp = seq[start + i];
                    seq[start + i] = other[otherStart + i];
                    other[otherStart + i] = temp;
                }
        }

        public static IMutableSeq<T> ToMutableSeq<T>(this IEnumerable<T> source)
        {
            return MutableSeq.Of(source);
        }

        public static IMutableSeq<T> Shuffle<T>(this IMutableSeq<T> seq)
        {
            return Shuffle(seq, RandomRegistry.GetRandom());
        }

        public static IMutableSeq<T> Shuffle<T>(this IMutableSeq<T> seq, Random randomSource)
        {
            for (var j = seq.Length - 1; j > 0; --j)
                seq.Swap(j, randomSource.NextInt(j + 1));
            return seq;
        }
    }
}