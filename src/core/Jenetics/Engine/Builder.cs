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
using System.Threading.Tasks;
using static Jenetics.Internal.Util.Require;

namespace Jenetics.Engine
{
    public class Builder<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly Func<Genotype<TGene>, TAllele> _fitnessFunction;
        private readonly Factory<Genotype<TGene>> _genotypeFactory;

        private IAlterer<TGene, TAllele> _alterer = Alterer.Of(
            new SinglePointCrossover<TGene, TAllele>(0.2),
            new Mutator<TGene, TAllele>(0.15)
        );

        private Func<TAllele, TAllele> _fitnessScaler = a => a;
        private int _individualCreationRetries = 10;
        private long _maximalPhenotypeAge = 70;
        private double _offspringFraction = 0.6;
        private ISelector<TGene, TAllele> _offspringSelector = new TournamentSelector<TGene, TAllele>(3);
        private Optimize _optimize = Jenetics.Optimize.Maximum;
        private int _populationSize = 50;
        private ISelector<TGene, TAllele> _survivorsSelector = new TournamentSelector<TGene, TAllele>(3);
        private TaskScheduler _taskScheduler = System.Threading.Tasks.TaskScheduler.Default;

        private Func<Phenotype<TGene, TAllele>, bool> _validator = p => p.IsValid;

        public Builder(Factory<Genotype<TGene>> genotypeFactory, Func<Genotype<TGene>, TAllele> fitnessFunction)
        {
            _fitnessFunction = fitnessFunction;
            _genotypeFactory = genotypeFactory;
        }

        public Engine<TGene, TAllele> Build()
        {
            return new Engine<TGene, TAllele>(_fitnessFunction, _genotypeFactory, _fitnessScaler, _survivorsSelector,
                _offspringSelector, _alterer, _validator, _optimize, GetOffspringCount(), GetSurvivorsCount(),
                _maximalPhenotypeAge, _taskScheduler, _individualCreationRetries);
        }

        public Builder<TGene, TAllele> Alterers(
            IAlterer<TGene, TAllele> first,
            params IAlterer<TGene, TAllele>[] rest
        )
        {
            _alterer = rest.Length == 0 ? first : Alterer.Of(rest).Compose(first);

            return this;
        }

        public Builder<TGene, TAllele> FitnessScaler(
            Func<TAllele, TAllele> scaler
        )
        {
            _fitnessScaler = scaler;
            return this;
        }

        public Builder<TGene, TAllele> MaximalPhenotypeAge(long age)
        {
            if (age < 1)
                throw new ArgumentException($"Phenotype age must be greater than one, but was {age}.");
            _maximalPhenotypeAge = age;
            return this;
        }

        public Builder<TGene, TAllele> OffspringFraction(double fraction)
        {
            _offspringFraction = Probability(fraction);
            return this;
        }

        public Builder<TGene, TAllele> PhenotypeValidator(
            Func<Phenotype<TGene, TAllele>, bool> validator
        )
        {
            _validator = validator;
            return this;
        }

        public Builder<TGene, TAllele> PopulationSize(int size)
        {
            if (size < 1)
                throw new ArgumentException($"Population size must be greater than zero, but was %{size}.");
            _populationSize = size;
            return this;
        }

        public Builder<TGene, TAllele> SurvivorsSize(int size)
        {
            if (size < 0)
                throw new ArgumentException($"Survivors must be greater or equal zero, but was {size}.");

            return SurvivorsFraction(size / (double) _populationSize);
        }

        public Builder<TGene, TAllele> Selector(ISelector<TGene, TAllele> selector)
        {
            _offspringSelector = NonNull(selector);
            _survivorsSelector = NonNull(selector);
            return this;
        }

        public Builder<TGene, TAllele> SurvivorsFraction(double fraction)
        {
            _offspringFraction = 1.0 - Probability(fraction);
            return this;
        }

        public Builder<TGene, TAllele> SurvivorsSelector(ISelector<TGene, TAllele> selector)
        {
            _survivorsSelector = selector;
            return this;
        }

        public Builder<TGene, TAllele> Minimizing()
        {
            return Optimize(Jenetics.Optimize.Minimum);
        }

        public Builder<TGene, TAllele> Maximizing()
        {
            return Optimize(Jenetics.Optimize.Maximum);
        }

        public Builder<TGene, TAllele> IndividualCreationRetries(int retries)
        {
            if (retries < 0)
                throw new ArgumentException($"Retry count must not be negative: {retries}");
            _individualCreationRetries = retries;
            return this;
        }

        public Builder<TGene, TAllele> GenotypeValidator(Func<Genotype<TGene>, bool> validator)
        {
            _validator = pt => validator(pt.GetGenotype());
            return this;
        }

        private int GetOffspringCount()
        {
            return (int) Math.Round(_offspringFraction * _populationSize);
        }

        private int GetSurvivorsCount()
        {
            return _populationSize - GetOffspringCount();
        }

        public Builder<TGene, TAllele> Optimize(Optimize optimize)
        {
            _optimize = optimize;
            return this;
        }

        public Builder<TGene, TAllele> OffspringSelector(ISelector<TGene, TAllele> selector)
        {
            _offspringSelector = selector;
            return this;
        }

        public Builder<TGene, TAllele> TaskScheduler(TaskScheduler executor)
        {
            _taskScheduler = NonNull(executor);
            return this;
        }
    }
}