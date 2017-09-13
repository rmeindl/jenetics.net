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
using Jenetics.Util;
using static Jenetics.Internal.Util.Require;

namespace Jenetics.Internal.Util
{
    public static class Equality
    {
        public static Func<Func<T, bool>, bool> Of<T>(T self, object other)
            where T : class
        {
            if (ReferenceEquals(self, other))
                return p => true;
            if (other == null || self.GetType() != other.GetType())
                return p => false;
            return p => p((T) other);
        }

        public static bool Eq<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            return a.SequenceEqual(b);
        }

        public static bool Eq(object a, object b)
        {
            return a != null ? a.Equals(b) : b == null;
        }

        public static bool Eq(double a, double b)
        {
            return a.CompareTo(b) == 0;
        }

        public static bool Eq(int a, int b)
        {
            return a.CompareTo(b) == 0;
        }

        public static bool Eq<T>(ISeq<T> a, ISeq<T> b)
        {
            return Seq.Equals(a, b);
        }

        public static bool OfType(object self, object other)
        {
            NonNull(self);
            return self == other ||
                   other != null && self.GetType() == other.GetType();
        }
    }
}