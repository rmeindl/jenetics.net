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

namespace Jenetics.Internal.Util
{
    public static class Require
    {
        public static double Probability(double p)
        {
            if (p < 0.0 || p > 1.0)
                throw new ArgumentOutOfRangeException($"The given probability is not in the range [0, 1]: {p}");
            return p;
        }

        public static double NonNegative(double value, string message = "Value")
        {
            if (value < 0)
                throw new ArgumentException($"{message} must not be negative: {value}.");
            return value;
        }

        public static int Positive(int value)
        {
            if (value <= 0)
                throw new ArgumentException($"Value is not positive: {value}");
            return value;
        }

        public static T NonNull<T>(T obj)
        {
            if (obj == null)
                throw new NullReferenceException();
            return obj;
        }

        public static T NonNull<T>(T obj, string message)
        {
            if (obj == null)
                throw new NullReferenceException(message);
            return obj;
        }
    }
}