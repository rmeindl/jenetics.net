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
using System.Linq;
using Jenetics.Engine;
using Jenetics.Internal.Math;
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics
{
    public class UniformCrossover<TGene, TAllele> : CrossoverBase<TGene, TAllele>
        where TGene : IGene<TAllele, TGene>
        where TAllele : IComparable<TAllele>
    {
        private readonly double _swapProbability;

        public UniformCrossover(double crossoverProbability, double swapProbability = Alterer.DefaultAlterProbability) :
            base(crossoverProbability)
        {
            _swapProbability = Require.Probability(swapProbability);
        }

        public UniformCrossover() : this(Alterer.DefaultAlterProbability)
        {
        }

        protected internal override int Crossover(IMutableSeq<TGene> that, IMutableSeq<TGene> other)
        {
            var length = Math.Min(that.Length, other.Length);
            return random
                .Indexes(RandomRegistry.GetRandom(), length, _swapProbability)
                .Peek(i => Swap(i, that, other))
                .Count();
        }

        private static void Swap<T>(
            int index,
            IMutableSeq<T> that,
            IMutableSeq<T> other
        )
        {
            var temp = that[index];
            that[index] = other[index];
            other[index] = temp;
        }
    }
}