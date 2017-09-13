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
using System.Linq;
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics
{
    [Serializable]
    public class Genotype<TGene> : IFactory<Genotype<TGene>>, IEnumerable<IChromosome<TGene>>, IVerifiable
        where TGene : IGene<TGene>
    {
        private readonly IImmutableSeq<IChromosome<TGene>> _chromosomes;
        private readonly int _ngenes;
        private bool? _valid;

        public Genotype(IImmutableSeq<IChromosome<TGene>> chromosomes) : this(chromosomes, Ngenes(chromosomes))
        {
        }

        private Genotype(IImmutableSeq<IChromosome<TGene>> chromosomes, int ngenes)
        {
            _chromosomes = chromosomes;
            _ngenes = ngenes;
        }

        public TGene Gene => _chromosomes[0].GetGene();

        public int Length => _chromosomes.Length;

        public IEnumerator<IChromosome<TGene>> GetEnumerator()
        {
            return _chromosomes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Genotype<TGene> NewInstance()
        {
            return new Genotype<TGene>(_chromosomes.Map(c => c.NewInstance()), _ngenes);
        }

        public bool IsValid
        {
            get
            {
                var valid = _valid;
                if (valid == null)
                {
                    valid = _chromosomes.ForAll(c => c.IsValid);
                    _valid = valid;
                }

                return _valid.HasValue && _valid.Value;
            }
        }

        private static int Ngenes(IImmutableSeq<IChromosome<TGene>> chromosomes)
        {
            return chromosomes.Select(c => c.Length).Sum();
        }

        public IChromosome<TGene> GetChromosome(int index)
        {
            return _chromosomes[index];
        }

        public IChromosome<TGene> GetChromosome()
        {
            return _chromosomes[0];
        }

        public int GetNumberOfGenes()
        {
            return _ngenes;
        }

        internal Genotype<TGene> NewInstance(IImmutableSeq<IChromosome<TGene>> chromosomes)
        {
            return new Genotype<TGene>(chromosomes, _ngenes);
        }

        public IImmutableSeq<IChromosome<TGene>> ToSeq()
        {
            return _chromosomes;
        }

        public override bool Equals(object obj)
        {
            return obj is Genotype<TGene> genotype && Equality.Eq(_chromosomes, genotype._chromosomes);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(_chromosomes).Value;
        }

        public override string ToString()
        {
            return _chromosomes.ToString();
        }
    }

    public static class Genotype
    {
        public static Genotype<TGene> Of<TGene>(IChromosome<TGene> first, params IChromosome<TGene>[] rest)
            where TGene : IGene<TGene>
        {
            var seq = MutableSeq.OfLength<IChromosome<TGene>>(1 + rest.Length);
            seq[0] = first;
            for (var i = 0; i < rest.Length; ++i)
                seq[i + 1] = rest[i];
            return new Genotype<TGene>(seq.ToImmutableSeq());
        }

        public static Genotype<TGene> Of<TGene>(IEnumerable<IChromosome<TGene>> chromosomes)
            where TGene : IGene<TGene>
        {
            return new Genotype<TGene>(ImmutableSeq.Of(chromosomes));
        }
    }
}