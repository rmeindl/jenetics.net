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
using System.Runtime.Serialization;
using Jenetics.Internal.Util;

namespace Jenetics
{
    [Serializable]
    public class Phenotype<TGene, TAllele> : IVerifiable, IComparable<Phenotype<TGene, TAllele>>, ISerializable
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        [NonSerialized] private readonly Lazy<TAllele> _fitness;
        [NonSerialized] private readonly Func<Genotype<TGene>, TAllele> _function;
        private readonly long _generation;
        private readonly Genotype<TGene> _genotype;
        [NonSerialized] private readonly Lazy<TAllele> _rawFitness;
        [NonSerialized] private readonly Func<TAllele, TAllele> _scaler;

        public Phenotype(Genotype<TGene> genotype, long generation, Func<Genotype<TGene>, TAllele> function,
            Func<TAllele, TAllele> scaler)
        {
            _genotype = genotype;
            _generation = generation;
            _function = function;
            _scaler = scaler;

            _rawFitness = new Lazy<TAllele>(() => _function(_genotype));
            _fitness = new Lazy<TAllele>(() => _scaler(_rawFitness.Value));
        }

        protected Phenotype(SerializationInfo info, StreamingContext context)
        {
            _genotype = (Genotype<TGene>) info.GetValue("_genotype", typeof(Genotype<TGene>));
            _generation = info.GetInt64("_generation");

            _rawFitness = new Lazy<TAllele>((TAllele) info.GetValue("_rawFitness", typeof(TAllele)));
            _fitness = new Lazy<TAllele>((TAllele) info.GetValue("_fitness", typeof(TAllele)));

            _function = a => default;
            _scaler = a => a;
        }

        public int CompareTo(Phenotype<TGene, TAllele> other)
        {
            return GetFitness().CompareTo(other.GetFitness());
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_genotype", _genotype);
            info.AddValue("_generation", _generation);

            info.AddValue("_rawFitness", _rawFitness.Value);
            info.AddValue("_fitness", _fitness.Value);
        }

        public bool IsValid => _genotype.IsValid;

        public Phenotype<TGene, TAllele> NewInstance(Genotype<TGene> genotype, long generation)
        {
            return Phenotype.Of(genotype, generation, _function, _scaler);
        }

        public Phenotype<TGene, TAllele> NewInstance(
            long generation,
            Func<Genotype<TGene>, TAllele> function,
            Func<TAllele, TAllele> scaler
        )
        {
            return Phenotype.Of(_genotype, generation, function, scaler);
        }

        public TAllele GetFitness()
        {
            return _fitness.Value;
        }

        public TAllele GetRawFitness()
        {
            return _rawFitness.Value;
        }

        public Phenotype<TGene, TAllele> Evaluate()
        {
            GetRawFitness();
            GetFitness();
            return this;
        }

        public long GetAge(long currentGeneration)
        {
            return currentGeneration - _generation;
        }

        public Genotype<TGene> GetGenotype()
        {
            return _genotype;
        }

        public long GetGeneration()
        {
            return _generation;
        }

        public override bool Equals(object obj)
        {
            return obj is Phenotype<TGene, TAllele> phenotype &&
                   Equality.Eq(GetFitness(), phenotype.GetFitness()) &&
                   Equality.Eq(GetRawFitness(), phenotype.GetRawFitness()) &&
                   Equality.Eq(_genotype, phenotype._genotype) &&
                   Equality.Eq(_generation, phenotype._generation);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType())
                .And(_generation)
                .And(GetFitness())
                .And(GetRawFitness())
                .And(_genotype).Value;
        }

        public override string ToString()
        {
            return _genotype + " --> " + GetFitness();
        }
    }

    public static class Phenotype
    {
        public static Phenotype<TGene, TAllele> Of<TGene, TAllele>(Genotype<TGene> genotype, long generation,
            Func<Genotype<TGene>, TAllele> function)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>
        {
            return new Phenotype<TGene, TAllele>(
                genotype,
                generation,
                function,
                a => a
            );
        }

        public static Phenotype<TGene, TAllele> Of<TGene, TAllele>(Genotype<TGene> genotype, long generation,
            Func<Genotype<TGene>, TAllele> function,
            Func<TAllele, TAllele> scaler)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>
        {
            return new Phenotype<TGene, TAllele>(
                genotype,
                generation,
                function,
                scaler
            );
        }
    }
}