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

using System;
using System.Linq;
using Jenetics.Engine;
using Xunit;

namespace Jenetics.Engine;

public class ComparableFitnessTest
{
    [Fact]
    public void EngineSupportsComparableFitnessWithoutNumericConversion()
    {
        var engine = Engine.Builder<BitGene, Fitness>(
            _ => new Fitness(1),
            () => Genotype.Of(BitChromosome.Of(2))
        )
        .PopulationSize(5)
        .Build();

        var results = engine.Stream().Take(2).ToArray();

        Assert.Equal(2, results.Length);
        Assert.Equal(new Fitness(1), results[1].GetBestFitness());
    }

    [Fact]
    public void FitnessThresholdSupportsComparableFitnessWithoutNumericConversion()
    {
        var result = Result(Optimize.Maximum, new Fitness(4));
        var limit = Limits.ByFitnessThreshold<BitGene, Fitness>(new Fitness(5));

        Assert.True(limit(result));
    }

    private static EvolutionResult<BitGene, Fitness> Result(
        Optimize optimize,
        Fitness fitness
    )
    {
        var genotype = Genotype.Of(BitChromosome.Of(2));
        var population = new Population<BitGene, Fitness>
        {
            Phenotype.Of(genotype, 1, _ => fitness),
        };
        return EvolutionResult.Of(
            optimize,
            population,
            1,
            EvolutionDurations.Zero,
            0,
            0,
            0
        );
    }

    private readonly record struct Fitness(int Value) : IComparable<Fitness>
    {
        public int CompareTo(Fitness other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}
