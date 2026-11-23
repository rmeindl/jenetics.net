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
using Jenetics.Engine;
using Xunit;

namespace Jenetics.Engine;

public class LimitsParityTest
{
    [Fact]
    public void FitnessThresholdUsesOptimizationDirection()
    {
        var maximumLimit = Limits.ByFitnessThreshold<BitGene, double>(10.0);
        Assert.True(maximumLimit(CreateResult(Optimize.Maximum, 9.0)));
        Assert.False(maximumLimit(CreateResult(Optimize.Maximum, 10.0)));
        Assert.False(maximumLimit(CreateResult(Optimize.Maximum, 11.0)));

        var minimumLimit = Limits.ByFitnessThreshold<BitGene, double>(5.0);
        Assert.True(minimumLimit(CreateResult(Optimize.Minimum, 6.0)));
        Assert.False(minimumLimit(CreateResult(Optimize.Minimum, 5.0)));
        Assert.False(minimumLimit(CreateResult(Optimize.Minimum, 4.0)));
    }

    [Fact]
    public void ExecutionTimeLimitHonorsDuration()
    {
        var result = CreateResult(Optimize.Maximum, 1.0);
        Assert.False(Limits.ByExecutionTime<BitGene, double>(TimeSpan.Zero)(result));
        Assert.True(Limits.ByExecutionTime<BitGene, double>(TimeSpan.FromHours(1))(result));
    }

    [Fact]
    public void FitnessConvergenceWaitsForLongWindow()
    {
        var predicateCalled = false;
        var limit = Limits.ByFitnessConvergence<BitGene, double>(
            1,
            2,
            (shortMoments, longMoments) =>
            {
                predicateCalled = true;
                Assert.Equal(1, shortMoments.Count);
                Assert.Equal(1.5, longMoments.Mean, 12);
                return false;
            }
        );

        Assert.True(limit(CreateResult(Optimize.Maximum, 1.0)));
        Assert.False(predicateCalled);
        Assert.False(limit(CreateResult(Optimize.Maximum, 2.0)));
        Assert.True(predicateCalled);
    }

    [Fact]
    public void PopulationConvergenceComparesBestWithPopulationMean()
    {
        var result = CreateResult(Optimize.Maximum, 10.0, 0.0);

        Assert.True(Limits.ByPopulationConvergence<BitGene, double>(0.4)(result));
        Assert.False(Limits.ByPopulationConvergence<BitGene, double>(0.6)(result));
    }

    [Fact]
    public void FixedGenerationLimitStopsAfterRequestedCount()
    {
        var limit = Limits.ByFixedGeneration<BitGene, double>(2);
        var result = CreateResult(Optimize.Maximum, 1.0);

        Assert.True(limit(result));
        Assert.True(limit(result));
        Assert.False(limit(result));
    }

    private static EvolutionResult<BitGene, double> CreateResult(
        Optimize optimize,
        params double[] fitnessValues
    )
    {
        var genotype = Genotype.Of(BitChromosome.Of(2));
        var population = new Population<BitGene, double>();
        foreach (var fitness in fitnessValues)
            population.Add(Phenotype.Of(genotype, 1, _ => fitness));

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
}
