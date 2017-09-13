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
using Jenetics.Util;

namespace Jenetics
{
    public class SinglePointCrossover<TGene, TAllele> : MultiPointCrossover<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        public SinglePointCrossover() : this(0.05)
        {
        }

        public SinglePointCrossover(double probability) : base(probability, 1)
        {
        }

        protected internal override int Crossover(IMutableSeq<TGene> that, IMutableSeq<TGene> other)
        {
            var random = RandomRegistry.GetRandom();

            var index = random.NextInt(Math.Min(that.Length, other.Length));
            SinglePointCrossover.Crossover(that, other, index);
            return 2;
        }
    }

    public static class SinglePointCrossover
    {
        internal static void Crossover<T>(IMutableSeq<T> that, IMutableSeq<T> other, int index)
        {
            that.Swap(index, Math.Min(that.Length, other.Length), other, index);
        }
    }
}