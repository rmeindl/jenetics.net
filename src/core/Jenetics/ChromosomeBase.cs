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
using System.Collections;
using System.Collections.Generic;
using Jenetics.Engine;
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics
{
    public interface IChromosome<TGene> : IVerifiable, IEnumerable<TGene>, IFactory<IChromosome<TGene>>
        where TGene : IGene<TGene>
    {
        int Length { get; }
        TGene GetGene();
        TGene GetGene(int index);

        IImmutableSeq<TGene> ToSeq();
        IChromosome<TGene> NewInstance(IImmutableSeq<TGene> genes);

        T As<T>();
    }

    [Serializable]
    public abstract class ChromosomeBase<TGene> : IChromosome<TGene>
        where TGene : IGene<TGene>, IVerifiable
    {
        protected IImmutableSeq<TGene> Genes;
        protected bool? Valid;

        // for serialization only
        protected internal ChromosomeBase()
        {
        }

        protected ChromosomeBase(IImmutableSeq<TGene> genes)
        {
            if (genes.Empty())
                throw new ArgumentException("The genes sequence must contain at least one gene.");
            Genes = genes;
        }

        public TGene GetGene()
        {
            return GetGene(0);
        }

        public TGene GetGene(int index)
        {
            return Genes[index];
        }

        public int Length => Genes.Length;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual bool IsValid
        {
            get
            {
                if (Valid == null)
                    Valid = Genes.ForAll(g => g.IsValid);

                return (bool) Valid;
            }
        }

        public IImmutableSeq<TGene> ToSeq()
        {
            return Genes;
        }

        public abstract IEnumerator<TGene> GetEnumerator();

        public abstract IChromosome<TGene> NewInstance();

        public abstract IChromosome<TGene> NewInstance(IImmutableSeq<TGene> genes);

        public T As<T>()
        {
            return (T) Convert.ChangeType(this, typeof(T));
        }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(ch => Equality.Eq(Genes, ch.Genes));
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(Genes).Value;
        }

        public override string ToString()
        {
            return Genes.ToString();
        }
    }
}