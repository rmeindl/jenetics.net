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
using Jenetics.Util;

namespace Jenetics
{
    public interface IBoundedChromosome<out TAllele, TGene> : IChromosome<TGene>
        where TAllele : IComparable<TAllele>
        where TGene : IBoundedGene<TAllele, TGene>
    {
        TAllele Min { get; }
        TAllele Max { get; }
    }

    [Serializable]
    public abstract class BoundedChromosomeBase<TAllele, TGene> : ChromosomeBase<TGene>,
        IBoundedChromosome<TAllele, TGene>
        where TAllele : IComparable<TAllele>
        where TGene : BoundedGeneBase<TAllele, TGene>
    {
        // for serialization only
        protected internal BoundedChromosomeBase()
        {
        }

        protected BoundedChromosomeBase(IImmutableSeq<TGene> genes) : base(genes)
        {
            Min = genes[0].Min;
            Max = genes[0].Max;
        }

        public TAllele Min { get; protected set; }

        public TAllele Max { get; protected set; }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(nc =>
                Equality.Eq(Min, nc.Min) &&
                Equality.Eq(Max, nc.Max) &&
                base.Equals(obj)
            );
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType())
                .And(base.GetHashCode())
                .And(Min)
                .And(Max).Value;
        }
    }
}