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

namespace Jenetics
{
    public enum Optimize
    {
        Minimum,
        Maximum
    }

    public static class OptimizeExtensions
    {
        public static IComparer<T> Ascending<T>(this Optimize opt)
            where T : IComparable<T>
        {
            return Comparer<T>.Create((x, y) => Compare(opt, x, y));
        }

        public static IComparer<T> Descending<T>(this Optimize opt)
            where T : IComparable<T>
        {
            return Comparer<T>.Create((x, y) => Compare(opt, y, x));
        }

        public static int Compare<T>(this Optimize opt, T a, T b)
            where T : IComparable<T>
        {
            return opt == Optimize.Maximum ? a.CompareTo(b) : b.CompareTo(a);
        }

        public static T Best<T>(this Optimize opt, T a, T b)
            where T : IComparable<T>
        {
            return Compare(opt, b, a) > 0 ? b : a;
        }

        public static T Worst<T>(this Optimize opt, T a, T b)
            where T : IComparable<T>
        {
            return Compare(opt, b, a) < 0 ? b : a;
        }
    }
}