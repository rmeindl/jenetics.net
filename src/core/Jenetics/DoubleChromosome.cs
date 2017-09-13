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
    public class DoubleChromosome : BoundedChromosomeBase<double, DoubleGene>,
        INumericChromosome<double, DoubleGene>
    {
        private DoubleChromosome(IImmutableSeq<DoubleGene> genes) : base(genes)
        {
        }

        public DoubleChromosome(double min, double max, int length = 1) : this(DoubleGene.Seq(min, max, length))
        {
            Valid = true;
        }

        public override IEnumerator<DoubleGene> GetEnumerator()
        {
            return Genes.GetEnumerator();
        }

        public override IChromosome<DoubleGene> NewInstance()
        {
            return new DoubleChromosome(Min, Max, Length);
        }

        public override IChromosome<DoubleGene> NewInstance(IImmutableSeq<DoubleGene> genes)
        {
            return new DoubleChromosome(genes);
        }

        public static DoubleChromosome Of(double min, double max)
        {
            return new DoubleChromosome(min, max);
        }

        public static DoubleChromosome Of(double min, double max, int length)
        {
            return new DoubleChromosome(min, max, length);
        }

        public static DoubleChromosome Of(DoubleRange range)
        {
            return new DoubleChromosome(range.Min, range.Max);
        }

        public static DoubleChromosome Of(params DoubleGene[] genes)
        {
            return new DoubleChromosome(ImmutableSeq.Of(genes));
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