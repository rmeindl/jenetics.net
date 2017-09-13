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
using System.Runtime.Serialization;
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics
{
    [Serializable]
    public class LongChromosome : BoundedChromosomeBase<long, LongGene>, INumericChromosome<long, LongGene>,
        ISerializable
    {
        private LongChromosome(IImmutableSeq<LongGene> genes) : base(genes)
        {
        }

        public LongChromosome(long min, long max, int length = 1) : this(LongGene.Seq(min, max, length))
        {
            Valid = true;
        }

        protected LongChromosome(SerializationInfo info, StreamingContext context)
        {
            var genes = MutableSeq.OfLength<LongGene>(info.GetInt32("_length"));
            Min = info.GetInt64("_min");
            Max = info.GetInt64("_max");

            for (var i = 0; i < genes.Length; ++i)
                genes[i] = new LongGene(info.GetInt64("_gene_" + i), Min, Max);

            Genes = genes.ToImmutableSeq();
        }

        public override IEnumerator<LongGene> GetEnumerator()
        {
            return Genes.GetEnumerator();
        }

        public override IChromosome<LongGene> NewInstance()
        {
            return new LongChromosome(Min, Max, Length);
        }

        public override IChromosome<LongGene> NewInstance(IImmutableSeq<LongGene> genes)
        {
            return new LongChromosome(genes);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_length", Length);
            info.AddValue("_min", Min);
            info.AddValue("_max", Max);

            for (var i = 0; i < Genes.Length; i++)
                info.AddValue("_gene_" + i, Genes[i].Allele);
        }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(base.Equals);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(base.GetHashCode()).Value;
        }

        public static LongChromosome Of(LongRange range)
        {
            return new LongChromosome(range.Min, range.Max);
        }

        public static LongChromosome Of(long min, long max, int length)
        {
            return new LongChromosome(min, max, length);
        }
    }
}