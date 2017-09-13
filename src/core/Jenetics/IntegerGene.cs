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
    public class IntegerGene : NumericGeneBase<int, IntegerGene>, IMean<IntegerGene>
    {
        public IntegerGene(int value, int min, int max) : base(value, min, max)
        {
        }

        public IntegerGene Mean(IntegerGene that)
        {
            return new IntegerGene(Value + (that.Value - Value) / 2, Min, Max);
        }

        public override int CompareTo(IntegerGene other)
        {
            return Value.CompareTo(other.Value);
        }

        public override IntegerGene NewInstance()
        {
            return new IntegerGene(RandomRegistry.GetRandom().NextInt(Min, Max), Min, Max);
        }

        public override IntegerGene NewInstance(int number)
        {
            return new IntegerGene(number, Min, Max);
        }

        internal static IImmutableSeq<IntegerGene> Seq(int minimum, int maximum, int length)
        {
            var min = minimum;
            var max = maximum;
            var r = RandomRegistry.GetRandom();

            return MutableSeq.OfLength<IntegerGene>(length)
                .Fill(() => new IntegerGene(r.NextInt(min, max), minimum, maximum))
                .ToImmutableSeq();
        }

        public static IntegerGene Of(int value, int min, int max)
        {
            return new IntegerGene(value, min, max);
        }

        public static IntegerGene Of(int min, int max)
        {
            return Of(random.NextInt(RandomRegistry.GetRandom(), min, max), min, max);
        }
    }
}