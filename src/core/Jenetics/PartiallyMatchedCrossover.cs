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
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics
{
    public class PartiallyMatchedCrossover<TGene, TAllele> : CrossoverBase<EnumGene<TGene>, TAllele>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        public PartiallyMatchedCrossover(double probability) : base(probability)
        {
        }

        protected internal override int Crossover(IMutableSeq<EnumGene<TGene>> that, IMutableSeq<EnumGene<TGene>> other)
        {
            if (that.Length != other.Length)
                throw new ArgumentException($"Required chromosomes with same length: {that.Length} != {other.Length}");

            if (that.Length >= 2)
            {
                var random = RandomRegistry.GetRandom();
                var points = Base.Subset(that.Length, 2, random);

                that.Swap(points[0], points[1], other, points[0]);
                Repair(that, other, points[0], points[1]);
                Repair(other, that, points[0], points[1]);
            }

            return 1;
        }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(base.Equals);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(base.GetHashCode()).Value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}[p={Probability}]";
        }

        private static void Repair<TT>(IMutableSeq<TT> that, IMutableSeq<TT> other, int begin, int end)
        {
            for (var i = 0; i < begin; ++i)
            {
                var index = that.IndexOf(that[i], begin, end);
                while (index != -1)
                {
                    that[i] = other[index];
                    index = that.IndexOf(that[i], begin, end);
                }
            }
            for (int i = end, n = that.Length; i < n; ++i)
            {
                var index = that.IndexOf(that[i], begin, end);
                while (index != -1)
                {
                    that[i] = other[index];
                    index = that.IndexOf(that[i], begin, end);
                }
            }
        }
    }
}