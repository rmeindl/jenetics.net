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
using System.Collections.Generic;
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics
{
    [Serializable]
    public class IntegerChromosome : BoundedChromosomeBase<int, IntegerGene>, INumericChromosome<int, IntegerGene>
    {
        private IntegerChromosome(IImmutableSeq<IntegerGene> genes) : base(genes)
        {
        }

        public IntegerChromosome(int min, int max, int length = 1) : this(IntegerGene.Seq(min, max, length))
        {
            Valid = true;
        }

        public override IEnumerator<IntegerGene> GetEnumerator()
        {
            return Genes.GetEnumerator();
        }

        public override IChromosome<IntegerGene> NewInstance()
        {
            return new IntegerChromosome(Min, Max, Length);
        }

        public override IChromosome<IntegerGene> NewInstance(IImmutableSeq<IntegerGene> genes)
        {
            return new IntegerChromosome(genes);
        }

        public static IntegerChromosome Of(int min, int max, int length
        )
        {
            return new IntegerChromosome(min, max, length);
        }

        public static IntegerChromosome Of(IntRange range)
        {
            return new IntegerChromosome(range.Min, range.Max);
        }

        public static IntegerChromosome Of(params IntegerGene[] genes)
        {
            return new IntegerChromosome(ImmutableSeq.Of(genes));
        }

        public static IntegerChromosome Of(int min, int max)
        {
            return new IntegerChromosome(min, max);
        }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(base.Equals);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(base.GetHashCode()).Value;
        }
    }
}