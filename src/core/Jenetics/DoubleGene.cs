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
using Jenetics.Internal.Math;
using Jenetics.Util;

namespace Jenetics
{
    [Serializable]
    public class DoubleGene : NumericGeneBase<double, DoubleGene>, IMean<DoubleGene>
    {
        public DoubleGene(double value, double min, double max) : base(value, min, max)
        {
        }

        public DoubleGene Mean(DoubleGene that)
        {
            return new DoubleGene(Value + (that.Value - Value) / 2.0, Min, Max);
        }

        public static IImmutableSeq<DoubleGene> Seq(double minimum, double maximum, int length)
        {
            var min = minimum;
            var max = maximum;
            var r = RandomRegistry.GetRandom();

            return MutableSeq.OfLength<DoubleGene>(length).Fill(() =>
                new DoubleGene(random.NextDouble(r, min, max), minimum, maximum)).ToImmutableSeq();
        }

        public override int CompareTo(DoubleGene other)
        {
            return Value.CompareTo(other.Value);
        }

        public override DoubleGene NewInstance()
        {
            var r = RandomRegistry.GetRandom();

            return new DoubleGene(random.NextDouble(r, Min, Max), Min, Max);
        }

        public override DoubleGene NewInstance(double number)
        {
            return new DoubleGene(number, Min, Max);
        }

        public static DoubleGene Of(double value, double min, double max)
        {
            return new DoubleGene(value, min, max);
        }

        public static DoubleGene Of(double min, double max)
        {
            return Of(random.NextDouble(RandomRegistry.GetRandom(), min, max), min, max);
        }
    }
}