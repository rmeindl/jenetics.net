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
    public class MultiPointCrossover<TGene, TAllele> : CrossoverBase<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly int _n;

        public MultiPointCrossover(double probability, int n = 2) : base(probability)
        {
            if (n < 1)
                throw new ArgumentOutOfRangeException($"n must be at least 1 but was {n}.");
            _n = n;
        }

        public MultiPointCrossover(int n) : this(0.05, n)
        {
        }

        public MultiPointCrossover() : this(0.05)
        {
        }

        protected internal override int Crossover(IMutableSeq<TGene> that, IMutableSeq<TGene> other)
        {
            var n = Math.Min(that.Length, other.Length);
            var k = Math.Min(n, _n);

            var random = RandomRegistry.GetRandom();
            var points = k > 0 ? Base.Subset(n, k, random) : new int[0];

            MultiPointCrossover.Crossover(that, other, points);
            return 2;
        }
    }

    public static class MultiPointCrossover
    {
        internal static void Crossover<T>(IMutableSeq<T> that, IMutableSeq<T> other, int[] indexes)
        {
            for (var i = 0; i < indexes.Length - 1; i += 2)
            {
                var start = indexes[i];
                var end = indexes[i + 1];
                that.Swap(start, end, other, start);
            }
            if (indexes.Length % 2 == 1)
            {
                var index = indexes[indexes.Length - 1];
                that.Swap(index, Math.Min(that.Length, other.Length), other, index);
            }
        }
    }
}