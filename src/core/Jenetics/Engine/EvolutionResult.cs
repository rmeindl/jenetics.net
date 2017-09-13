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
using System.Runtime.Serialization;

namespace Jenetics.Engine
{
    public static class EvolutionResult
    {
        public static EvolutionResult<TGene, TAllele> Of<TGene, TAllele>(
            Optimize optimize,
            Population<TGene, TAllele> population,
            long generation,
            EvolutionDurations durations,
            int killCount,
            int invalidCount,
            int alterCount
        )
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return new EvolutionResult<TGene, TAllele>(
                optimize,
                population,
                generation,
                generation,
                durations,
                killCount,
                invalidCount,
                alterCount
            );
        }

        public static EvolutionResult<TGene, TAllele> Of<TGene, TAllele>(
            Optimize optimize,
            Population<TGene, TAllele> population,
            long generation,
            long totalGenerations,
            EvolutionDurations durations,
            int killCount,
            int invalidCount,
            int alterCount
        )
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return new EvolutionResult<TGene, TAllele>(
                optimize,
                population,
                generation,
                totalGenerations,
                durations,
                killCount,
                invalidCount,
                alterCount
            );
        }

        public static EvolutionResult<TGene, TAllele> ToBestEvolutionResult<TGene, TAllele>(
            this IEnumerable<EvolutionResult<TGene, TAllele>> source)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            var count = 0;
            EvolutionResult<TGene, TAllele> bestResult = null;
            foreach (var result in source)
            {
                if (bestResult == null)
                {
                    bestResult = result;
                }
                else
                {
                    if (result.GetOptimize().Compare(result.GetBestPhenotype(), bestResult.GetBestPhenotype()) > 0)
                        bestResult = result;
                }
                count++;
            }

            return bestResult?.WithTotalGenerations(count);
        }

        public static Phenotype<TGene, TAllele> ToBestPhenotype<TGene, TAllele>(
            this IEnumerable<EvolutionResult<TGene, TAllele>> source)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            Phenotype<TGene, TAllele> bestPhenotype = null;
            foreach (var result in source)
                if (bestPhenotype == null)
                {
                    bestPhenotype = result.GetBestPhenotype();
                }
                else
                {
                    if (result.GetOptimize()
                            .Compare(result.GetBestPhenotype().GetFitness(), bestPhenotype.GetFitness()) > 0)
                        bestPhenotype = result.GetBestPhenotype();
                }

            return bestPhenotype;
        }

        public static Genotype<TGene> ToBestGenotype<TGene, TAllele>(
            this IEnumerable<EvolutionResult<TGene, TAllele>> source)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return ToBestPhenotype(source)?.GetGenotype();
        }
    }

    [Serializable]
    public class EvolutionResult<TGene, TAllele> : IComparable<EvolutionResult<TGene, TAllele>>, ISerializable
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly int _alterCount;

        [NonSerialized] private readonly Lazy<Phenotype<TGene, TAllele>> _best;

        private readonly EvolutionDurations _durations;
        private readonly long _generation;
        private readonly int _invalidCount;
        private readonly int _killCount;
        private readonly Optimize _optimize;
        private readonly Population<TGene, TAllele> _population;
        private readonly long _totalGenerations;
        [NonSerialized] private readonly Lazy<Phenotype<TGene, TAllele>> _worst;

        public EvolutionResult(
            Optimize optimize,
            Population<TGene, TAllele> population,
            long generation,
            long totalGenerations,
            EvolutionDurations durations,
            int killCount,
            int invalidCount,
            int alterCount
        )
        {
            _optimize = optimize;
            _population = population;
            _generation = generation;
            _totalGenerations = totalGenerations;
            _durations = durations;
            _killCount = killCount;
            _invalidCount = invalidCount;
            _alterCount = alterCount;

            _best = new Lazy<Phenotype<TGene, TAllele>>(() =>
                _population.OrderBy(p => p, _optimize.Descending<Phenotype<TGene, TAllele>>()).FirstOrDefault());

            _worst = new Lazy<Phenotype<TGene, TAllele>>(() =>
                _population.OrderBy(p => p, _optimize.Ascending<Phenotype<TGene, TAllele>>()).FirstOrDefault());
        }

        protected EvolutionResult(SerializationInfo info, StreamingContext context)
        {
            _optimize = (Optimize) info.GetValue("_optimize", typeof(Optimize));
            _population = (Population<TGene, TAllele>) info.GetValue("_population", typeof(Population<TGene, TAllele>));
            _generation = info.GetInt64("_generation");
            _totalGenerations = info.GetInt64("_totalGenerations");
            _durations = (EvolutionDurations) info.GetValue("_durations", typeof(EvolutionDurations));
            _killCount = info.GetInt32("_killCount");
            _invalidCount = info.GetInt32("_invalidCount");
            _alterCount = info.GetInt32("_alterCount");

            _best = new Lazy<Phenotype<TGene, TAllele>>(
                (Phenotype<TGene, TAllele>) info.GetValue("_best", typeof(Phenotype<TGene, TAllele>)));
            _worst = new Lazy<Phenotype<TGene, TAllele>>(
                (Phenotype<TGene, TAllele>) info.GetValue("_worst", typeof(Phenotype<TGene, TAllele>)));
        }

        public int CompareTo(EvolutionResult<TGene, TAllele> other)
        {
            return _optimize.Compare(_best.Value, other._best.Value);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_optimize", _optimize);
            info.AddValue("_population", _population);
            info.AddValue("_generation", _generation);
            info.AddValue("_totalGenerations", _totalGenerations);
            info.AddValue("_durations", _durations);
            info.AddValue("_killCount", _killCount);
            info.AddValue("_invalidCount", _invalidCount);
            info.AddValue("_alterCount", _alterCount);

            info.AddValue("_best", _best.Value);
            info.AddValue("_worst", _best.Value);
        }

        public int GetKillCount()
        {
            return _killCount;
        }

        public int GetAlterCount()
        {
            return _alterCount;
        }

        public long GetGeneration()
        {
            return _generation;
        }

        internal EvolutionStart<TGene, TAllele> Next()
        {
            return EvolutionStart.Of(_population, _generation + 1);
        }

        public Phenotype<TGene, TAllele> GetBestPhenotype()
        {
            return _best.Value;
        }

        public Phenotype<TGene, TAllele> GetWorstPhenotype()
        {
            return _worst.Value;
        }

        public EvolutionDurations GetDurations()
        {
            return _durations;
        }

        public Population<TGene, TAllele> GetPopulation()
        {
            return _population.Copy();
        }

        public TAllele GetBestFitness()
        {
            return _best.Value != null ? _best.Value.GetFitness() : default;
        }

        public TAllele GetWorstFitness()
        {
            return _worst.Value != null ? _worst.Value.GetFitness() : default;
        }

        public Optimize GetOptimize()
        {
            return _optimize;
        }

        public long GetTotalGenerations()
        {
            return _totalGenerations;
        }

        public int GetInvalidCount()
        {
            return _invalidCount;
        }

        public override bool Equals(object obj)
        {
            return obj is EvolutionResult<TGene, TAllele> result &&
                   Equals(_optimize,
                       result._optimize) &&
                   Equals(_population,
                       result._population) &&
                   Equals(_generation,
                       result._generation) &&
                   Equals(_totalGenerations,
                       result._totalGenerations) &&
                   Equals(_durations,
                       result._durations) &&
                   Equals(_killCount,
                       result._killCount) &&
                   Equals(_invalidCount,
                       result._invalidCount) &&
                   Equals(_alterCount,
                       result._alterCount) &&
                   Equals(GetBestFitness(),
                       result.GetBestFitness()
                   );
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash += 31 * _optimize.GetHashCode() + 17;
            hash += 31 * _population.GetHashCode() + 17;
            hash += 31 * _generation.GetHashCode() + 17;
            hash += 31 * _totalGenerations.GetHashCode() + 17;
            hash += 31 * _durations.GetHashCode() + 17;
            hash += 31 * _killCount.GetHashCode() + 17;
            hash += 31 * _invalidCount.GetHashCode() + 17;
            hash += 31 * _alterCount.GetHashCode() + 17;
            hash += 31 * GetBestFitness().GetHashCode() + 17;
            return hash;
        }

        internal EvolutionResult<TGene, TAllele> WithTotalGenerations(long total)
        {
            return EvolutionResult.Of(
                _optimize,
                _population,
                _generation,
                total,
                _durations,
                _killCount,
                _invalidCount,
                _alterCount
            );
        }
    }
}