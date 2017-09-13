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
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics
{
    public class SwapMutator<TGene, TAllele> : Mutator<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        public SwapMutator() : this(Alterer.DefaultAlterProbability)
        {
        }

        public SwapMutator(double probability) : base(probability)
        {
        }

        protected override int Mutate(IMutableSeq<TGene> genes, double p)
        {
            var random = RandomRegistry.GetRandom();

            if (genes.Length > 1)
                return Internal.Math.random.Indexes(random, genes.Length, p)
                    .Peek(i => genes.Swap(i, random.NextInt(genes.Length)))
                    .Count();
            return 0;
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
    }
}