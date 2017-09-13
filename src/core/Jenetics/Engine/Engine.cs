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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jenetics.Util;

namespace Jenetics.Engine
{
    public static class Engine
    {
        public static Builder<TGene, TAllele> Builder<TGene, TAllele>(Func<Genotype<TGene>, TAllele> ff,
            Factory<Genotype<TGene>> genotypeFactory)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return new Builder<TGene, TAllele>(genotypeFactory, ff);
        }

        public static Builder<TGene, TAllele> Builder<T, TGene, TAllele>(Func<T, TAllele> ff, ICodec<T, TGene> codec)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return Builder(ff.Compose(codec.Decoder()), codec.Encoding());
        }

        public static Builder<TGene, TAllele> Builder<T, TGene, TAllele>(IProblem<T, TGene, TAllele> problem)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return Builder(problem.Fitness(), problem.Codec());
        }

        public static Builder<TGene, TAllele> Builder<TGene, TAllele>(Func<Genotype<TGene>, TAllele> ff,
            IChromosome<TGene> chromosome,
            params IChromosome<TGene>[] chromosomes)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return new Builder<TGene, TAllele>(() => Genotype.Of(chromosome, chromosomes), ff);
        }
    }

    public class Engine<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly IAlterer<TGene, TAllele> _alterer;
        private readonly Func<Genotype<TGene>, TAllele> _fitnessFunction;

        private readonly Func<TAllele, TAllele> _fitnessScaler;
        private readonly Factory<Genotype<TGene>> _genotypeFactory;

        private readonly int _individualCreationRetries;
        private readonly long _maximalPhenotypeAge;
        private readonly int _offspringCount;
        private readonly ISelector<TGene, TAllele> _offspringSelector;
        private readonly Optimize _optimize;
        private readonly int _survivorsCount;
        private readonly ISelector<TGene, TAllele> _survivorsSelector;

        private readonly TaskScheduler _taskScheduler;
        private readonly Func<Phenotype<TGene, TAllele>, bool> _validator;

        internal Engine(
            Func<Genotype<TGene>, TAllele> fitnessFunction,
            Factory<Genotype<TGene>> genotypeFactory,
            Func<TAllele, TAllele> fitnessScaler,
            ISelector<TGene, TAllele> survivorsSelector,
            ISelector<TGene, TAllele> offspringSelector,
            IAlterer<TGene, TAllele> alterer,
            Func<Phenotype<TGene, TAllele>, bool> validator,
            Optimize optimize,
            int offspringCount,
            int survivorsCount,
            long maximalPhenotypeAge,
            TaskScheduler taskScheduler,
            int individualCreationRetries
        )
        {
            _fitnessFunction = fitnessFunction;
            _genotypeFactory = genotypeFactory;
            _fitnessScaler = fitnessScaler;
            _survivorsSelector = survivorsSelector;
            _offspringSelector = offspringSelector;
            _alterer = alterer;
            _validator = validator;
            _optimize = optimize;

            _offspringCount = offspringCount;
            _survivorsCount = survivorsCount;
            _maximalPhenotypeAge = maximalPhenotypeAge;

            _taskScheduler = taskScheduler;

            _individualCreationRetries = individualCreationRetries;
        }

        public Builder<TGene, TAllele> Builder()
        {
            return new Builder<TGene, TAllele>(_genotypeFactory, _fitnessFunction)
                .Alterers(_alterer)
                .FitnessScaler(_fitnessScaler)
                .MaximalPhenotypeAge(_maximalPhenotypeAge)
                .OffspringFraction(_offspringCount / (double) GetPopulationSize())
                .OffspringSelector(_offspringSelector)
                .Optimize(_optimize)
                .PhenotypeValidator(_validator)
                .PopulationSize(GetPopulationSize())
                .SurvivorsSelector(_survivorsSelector)
                .IndividualCreationRetries(_individualCreationRetries);
        }

        private EvolutionStart<TGene, TAllele> EvolutionStart()
        {
            const int generation = 1;
            var size = _offspringCount + _survivorsCount;

            var population = new Population<TGene, TAllele>(size).Fill(() => NewPhenotype(generation), size);

            return Jenetics.Engine.EvolutionStart.Of(population, generation);
        }

        public int GetPopulationSize()
        {
            return _offspringCount + _survivorsCount;
        }

        private EvolutionStart<TGene, TAllele> EvolutionStart(IEnumerable<Genotype<TGene>> genotypes, long generation)
        {
            var pt = genotypes.Select(gt => Phenotype.Of(gt, generation, _fitnessFunction, _fitnessScaler));

            IEnumerable<Phenotype<TGene, TAllele>> F()
            {
                while (true)
                    yield return NewPhenotype(generation);
            }

            var stream = pt.Concat(F());

            var population = stream
                .Take(GetPopulationSize())
                .ToPopulation();

            return Jenetics.Engine.EvolutionStart.Of(population, generation);
        }

        private EvolutionStart<TGene, TAllele> EvolutionStart(
            Population<TGene, TAllele> population,
            long generation
        )
        {
            var pt = population.Select(p => p.NewInstance(p.GetGeneration(), _fitnessFunction, _fitnessScaler));

            IEnumerable<Phenotype<TGene, TAllele>> F()
            {
                while (true)
                    yield return NewPhenotype(generation);
            }

            var stream = pt.Concat(F());

            var pop = stream
                .Take(GetPopulationSize())
                .ToPopulation();

            return Jenetics.Engine.EvolutionStart.Of(pop, generation);
        }

        public IEnumerable<EvolutionResult<TGene, TAllele>> Stream()
        {
            return new EvolutionIterator<TGene, TAllele>(EvolutionStart, Evolve, CancellationToken.None);
        }

        public IEnumerable<EvolutionResult<TGene, TAllele>> Stream(CancellationToken cancellationToken)
        {
            return new EvolutionIterator<TGene, TAllele>(EvolutionStart, Evolve, cancellationToken);
        }

        public IEnumerable<EvolutionResult<TGene, TAllele>> Stream(EvolutionResult<TGene, TAllele> result)
        {
            return Stream(result.GetPopulation(), result.GetTotalGenerations(), CancellationToken.None);
        }

        public IEnumerable<EvolutionResult<TGene, TAllele>> Stream(EvolutionResult<TGene, TAllele> result,
            CancellationToken cancellationToken)
        {
            return Stream(result.GetPopulation(), result.GetTotalGenerations(), cancellationToken);
        }

        public IEnumerable<EvolutionResult<TGene, TAllele>> Stream(
            Population<TGene, TAllele> population,
            long generation
        )
        {
            return new EvolutionIterator<TGene, TAllele>(() => EvolutionStart(population, generation), Evolve,
                CancellationToken.None);
        }

        public IEnumerable<EvolutionResult<TGene, TAllele>> Stream(
            Population<TGene, TAllele> population,
            long generation,
            CancellationToken cancellationToken
        )
        {
            return new EvolutionIterator<TGene, TAllele>(() => EvolutionStart(population, generation), Evolve,
                cancellationToken);
        }

        public IEnumerable<EvolutionResult<TGene, TAllele>> Stream(IEnumerable<Genotype<TGene>> genotypes,
            CancellationToken cancellationToken)
        {
            return new EvolutionIterator<TGene, TAllele>(() => EvolutionStart(genotypes, 1), Evolve, cancellationToken);
        }

        public IEnumerable<EvolutionResult<TGene, TAllele>> Stream(IEnumerable<Genotype<TGene>> genotypes)
        {
            return new EvolutionIterator<TGene, TAllele>(() => EvolutionStart(genotypes, 1), Evolve,
                CancellationToken.None);
        }

        private Phenotype<TGene, TAllele> NewPhenotype(long generation)
        {
            var count = 0;
            Phenotype<TGene, TAllele> phenotype;

            do
            {
                phenotype = Phenotype.Of(
                    _genotypeFactory(),
                    generation,
                    _fitnessFunction,
                    _fitnessScaler
                );
            } while (++count < _individualCreationRetries &&
                     !_validator(phenotype));

            return phenotype;
        }

        public EvolutionResult<TGene, TAllele> Evolve(EvolutionStart<TGene, TAllele> start,
            CancellationToken cancellationToken)
        {
            var timer = Timer.Of().Start();

            var startPopulation = start.Population;

            var evaluateTimer = Timer.Of().Start();
            Evaluate(startPopulation, cancellationToken);
            evaluateTimer.Stop();

            var taskFactory = new TaskFactory(cancellationToken, TaskCreationOptions.None, TaskContinuationOptions.None,
                _taskScheduler);

            var offspring = taskFactory.StartNewTimed(() => SelectOffspring(startPopulation));

            var survivors = taskFactory.StartNewTimed(() => SelectSurvivors(startPopulation));

            var alteredOffspring = offspring.ContinueWithTimed(t => Alter(t.Result.Result, start.Generation));

            var filteredSurvivors =
                survivors.ContinueWithTimed(t => Filter(t.Result.Result, start.Generation));

            var filteredOffspring =
                alteredOffspring.ContinueWithTimed(t => Filter(t.Result.Result.Population, start.Generation));

            var population = taskFactory.ContinueWhenAll(
                new[] {filteredSurvivors, filteredOffspring},
                tasks =>
                {
                    var pop = new Population<TGene, TAllele>(filteredSurvivors.Result.Result.Population.Count +
                                                             filteredOffspring.Result.Result.Population.Count);
                    pop.AddAll(filteredSurvivors.Result.Result.Population);
                    pop.AddAll(filteredOffspring.Result.Result.Population);
                    return pop;
                });

            population.Wait(cancellationToken);

            var result = TimedResult.Of(() => Evaluate(population.Result, cancellationToken))();

            var durations = EvolutionDurations.Of(
                offspring.Result.Duration,
                survivors.Result.Duration,
                alteredOffspring.Result.Duration,
                filteredOffspring.Result.Duration,
                filteredSurvivors.Result.Duration,
                result.Duration.Add(evaluateTimer.GetTime()),
                timer.Stop().GetTime()
            );

            var killCount =
                filteredOffspring.Result.Result.KillCount +
                filteredSurvivors.Result.Result.KillCount;

            var invalidCount =
                filteredOffspring.Result.Result.InvalidCount +
                filteredSurvivors.Result.Result.InvalidCount;

            return EvolutionResult.Of(
                _optimize,
                result.Result,
                start.Generation,
                durations,
                killCount,
                invalidCount,
                alteredOffspring.Result.Result.AlterCount
            );
        }

        private Population<TGene, TAllele> SelectOffspring(Population<TGene, TAllele> population)
        {
            return _offspringCount > 0
                ? _offspringSelector.Select(population, _offspringCount, _optimize)
                : Population.Empty<TGene, TAllele>();
        }

        private Population<TGene, TAllele> SelectSurvivors(Population<TGene, TAllele> population)
        {
            return _survivorsCount > 0
                ? _survivorsSelector.Select(population, _survivorsCount, _optimize)
                : Population.Empty<TGene, TAllele>();
        }

        private AlterResult<TGene, TAllele> Alter(Population<TGene, TAllele> population, long generation)
        {
            return new AlterResult<TGene, TAllele>(population, _alterer.Alter(population, generation));
        }

        private Population<TGene, TAllele> Evaluate(Population<TGene, TAllele> population,
            CancellationToken cancellationToken)
        {
            var options = new ParallelOptions {TaskScheduler = _taskScheduler, CancellationToken = cancellationToken};
            Parallel.ForEach(population, options, phenotype => phenotype.Evaluate());
            return population;
        }

        private FilterResult<TGene, TAllele> Filter(Population<TGene, TAllele> population, long generation)
        {
            var killCount = 0;
            var invalidCount = 0;

            for (int i = 0, n = population.Count; i < n; ++i)
            {
                var individual = population[i];

                if (!_validator(individual))
                {
                    population[i] = NewPhenotype(generation);
                    ++invalidCount;
                }
                else if (individual.GetAge(generation) > _maximalPhenotypeAge)
                {
                    population[i] = NewPhenotype(generation);
                    ++killCount;
                }
            }

            return new FilterResult<TGene, TAllele>(population, killCount, invalidCount);
        }
    }

    public static class Extensions
    {
        public static Func<T, TReturn2> Compose<T, TReturn1, TReturn2>(this Func<TReturn1, TReturn2> func1,
            Func<T, TReturn1> func2)
        {
            return x => func1(func2(x));
        }

        public static Population<TGene, TAllele> ToPopulation<TGene, TAllele>(
            this IEnumerable<Phenotype<TGene, TAllele>> source)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            var population = new Population<TGene, TAllele>();
            population.AddAll(source);
            return population;
        }

        public static bool Empty<TSource>(this ISeq<TSource> seq)
        {
            return seq.Length == 0;
        }

        public static Task<TimedResult<T>> StartNewTimed<T>(this TaskFactory factory, Func<T> supplier)
        {
            var timed = TimedResult.Of(supplier);
            return factory.StartNew(timed);
        }

        public static Task<TimedResult<TNewResult>> ContinueWithTimed<TResult, TNewResult>(this Task<TResult> task,
            Func<Task<TResult>, TNewResult> supplier)
        {
            var timed = TimedResult.Of(() => supplier(task));
            return task.ContinueWith(t => timed());
        }

        public static IEnumerable<T> Peek<T>(
            this IEnumerable<T> source, Action<T> action)
        {
            foreach (var r in source)
            {
                action(r);
                yield return r;
            }
        }
    }
}