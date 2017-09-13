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
using Jenetics.Internal.Util;

namespace Jenetics
{
    [Serializable]
    public class ExponentialScaler
    {
        public static ExponentialScaler SqrScaler = new ExponentialScaler(2);
        public static ExponentialScaler SqrtScaler = new ExponentialScaler(0.5);

        private readonly double _a;
        private readonly double _b;
        private readonly double _c;

        public ExponentialScaler(double a, double b, double c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        public ExponentialScaler(double b, double c) : this(1.0, b, c)
        {
        }

        public ExponentialScaler(double c) : this(1.0, 0.0, c)
        {
        }

        public double Apply(double value)
        {
            return Math.Pow(_a * value + _b, _c);
        }

        public override bool Equals(object obj)
        {
            return obj is ExponentialScaler scaler &&
                   Equality.Eq(scaler._a, _a) &&
                   Equality.Eq(scaler._b, _b) &&
                   Equality.Eq(scaler._c, _c);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType())
                .And(_a)
                .And(_b)
                .And(_c).Value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}[a={_a}, b={_b}, c={_c}]";
        }
    }
}