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

namespace Jenetics.Internal.Math
{
    [Serializable]
    public class DoubleAdder : IComparable<DoubleAdder>
    {
        private double _compensation;
        private double _simpleSum;
        private double _sum;

        public DoubleAdder()
        {
        }

        public DoubleAdder(double value)
        {
            Add(value);
        }

        public double Value
        {
            get
            {
                var result = _sum + _compensation;
                return double.IsNaN(result) && double.IsInfinity(_simpleSum) ? _simpleSum : result;
            }
        }

        public double DoubleValue => Value;

        public int CompareTo(DoubleAdder other)
        {
            return DoubleValue.CompareTo(other.DoubleValue);
        }

        public DoubleAdder Add(double value)
        {
            AddWithCompensation(value);
            _simpleSum += value;
            return this;
        }

        public DoubleAdder Add(double[] values)
        {
            for (var i = values.Length; --i >= 0;)
                Add(values[i]);

            return this;
        }

        private void AddWithCompensation(double value)
        {
            var y = value - _compensation;
            var t = _sum + y;
            _compensation = t - _sum - y;
            _sum = t;
        }

        public static double Sum(double[] values)
        {
            return new DoubleAdder().Add(values).DoubleValue;
        }

        public bool SameState(DoubleAdder other)
        {
            return _sum.CompareTo(other._sum) == 0 &&
                   _simpleSum.CompareTo(other._simpleSum) == 0 &&
                   _compensation.CompareTo(other._compensation) == 0;
        }

        public override bool Equals(object obj)
        {
            return obj is DoubleAdder adder &&
                   Equality.Eq(DoubleValue, adder.DoubleValue);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(DoubleValue).Value;
        }

        public override string ToString()
        {
            return $"{DoubleValue}";
        }
    }
}