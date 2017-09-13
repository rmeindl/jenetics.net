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
    public interface IImmutableSeq<T> : ISeq<T>, ICopyable<IMutableSeq<T>>
    {
        IImmutableSeq<TR> Map<TR>(Func<T, TR> mapper);

        new IImmutableSeq<T> SubSeq(int start, int end);
        new IImmutableSeq<T> SubSeq(int start);
    }

    public static class ImmutableSeq
    {
        public static IImmutableSeq<T> Empty<T>()
        {
            return Internal.Collection.Empty.ImmutableSeq<T>();
        }

        public static IImmutableSeq<T> Of<T>(params T[] values)
        {
            return values.Length == 0 ? Empty<T>() : MutableSeq.Of(values).ToImmutableSeq();
        }

        public static IImmutableSeq<T> Of<T>(IEnumerable<T> values)
        {
            return values is IImmutableSeq<T> of
                ? of
                : values is IMutableSeq<T>
                    ? ((IMutableSeq<T>) values).ToImmutableSeq()
                    : MutableSeq.Of(values).ToImmutableSeq();
        }

        public static IImmutableSeq<T> ToImmutableSeq<T>(this IEnumerable<T> source)
        {
            return Of(source);
        }
    }
}