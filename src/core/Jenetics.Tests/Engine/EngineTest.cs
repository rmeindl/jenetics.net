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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Jenetics.Util;
using Xunit;

namespace Jenetics.Engine
{
    public class EngineTest
    {
        [Theory]
        [ClassData(typeof(GenerationsDataGenerator))]
        public void GenerationCount(long generations)
        {
            var engine = Engine
                .Builder(a => a.Gene.Allele, DoubleChromosome.Of(0, 1))
                .Build();

            var result = engine.Stream()
                .Take((int) generations)
                .ToBestEvolutionResult();

            Assert.Equal(generations, result.GetTotalGenerations());
        }

        [Theory]
        [ClassData(typeof(GenerationsDataGenerator))]
        public void GenerationLimit(long generations)
        {
            var engine = Engine
                .Builder(a => a.Gene.Allele, DoubleChromosome.Of(0, 1))
                .Build();

            var result = engine.Stream()
                .TakeWhile(Limits.ByFixedGeneration<DoubleGene, double>(generations))
                .ToBestEvolutionResult();

            Assert.Equal(generations, result.GetTotalGenerations());
        }

        private class GenerationsDataGenerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                for (var i = 1; i <= 10; i++)
                    yield return new object[] {i};
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Fact]
        public void GenotypeValidator()
        {
            const int populationSize = 100;

            var engine = Engine
                .Builder(a => a.Gene.Allele, DoubleChromosome.Of(0, 1))
                .GenotypeValidator(pt => false)
                .PopulationSize(populationSize)
                .Build();

            var result = engine.Stream()
                .Take(10)
                .ToBestEvolutionResult();

            Assert.Equal(result.GetInvalidCount(), populationSize);
        }

        [Fact]
        public void InitialResult()
        {
            var problem = Problem.Of(
                x => Math.Cos(0.5 + Math.Sin(x)) * Math.Cos(x),
                Codecs.OfScalar(DoubleRange.Of(0.0, 2.0 * Math.PI))
            );

            var engine = Engine.Builder(problem)
                .Optimize(Optimize.Minimum)
                .OffspringSelector(new RouletteWheelSelector<DoubleGene, double>())
                .Build();

            var interimResult = engine.Stream()
                .TakeWhile(Limits.BySteadyFitness<DoubleGene, double>(10))
                .ToBestEvolutionResult();

            engine.Builder()
                .Alterers(new Mutator<DoubleGene, double>()).Build()
                .Stream(interimResult);
        }

        [Fact]
        public void ParallelStream()
        {
            var engine = Engine
                .Builder(a => a.Gene.Allele, DoubleChromosome.Of(0, 1))
                .Build();

            var result = engine
                .Stream()
                .AsParallel()
                .TakeWhile(Limits.ByFixedGeneration<DoubleGene, double>(1000))
                .ToBestEvolutionResult();

            Assert.True(result.GetTotalGenerations() >= 1000,
                $"Total generation must be bigger than 1000: {result.GetTotalGenerations()}");
        }

        [Fact]
        public void PhenotypeValidator()
        {
            const int populationSize = 100;

            var engine = Engine
                .Builder(a => a.Gene.Allele, DoubleChromosome.Of(0, 1))
                .PhenotypeValidator(pt => false)
                .PopulationSize(populationSize)
                .Build();

            var result = engine.Stream()
                .Take(10)
                .ToBestEvolutionResult();

            Assert.Equal(result.GetInvalidCount(), populationSize);
        }

        [Fact]
        public void StreamWithInitialGenotypes()
        {
            var problem = Problem.Of(
                a => a,
                Codec.Of(
                    () => Genotype.Of(IntegerChromosome.Of(0, 1000)),
                    g => g.Gene.Allele
                )
            );

            const int genotypeCount = 10;
            const int max = 1000;
            var genotypes = IntRange.Of(1, genotypeCount)
                .Select(i => IntegerChromosome.Of(IntegerGene.Of(max, 0, max)))
                .Select(i => Genotype.Of(i))
                .ToImmutableSeq();

            var engine = Engine.Builder(problem).Build();

            var result = engine.Stream(genotypes)
                .Take(1)
                .ToBestEvolutionResult();

            long maxCount = result.GetPopulation().Count(pt => pt.GetFitness() == max);

            Assert.True(maxCount >= genotypeCount, $"{maxCount} >= {genotypeCount}");
        }

        [Fact]
        public void StreamWithSerializedPopulation()
        {
            var problem = Problem.Of(
                x => Math.Cos(0.5 + Math.Sin(x)) * Math.Cos(x),
                Codecs.OfScalar(DoubleRange.Of(0.0, 2.0 * Math.PI))
            );

            var engine = Engine.Builder(problem)
                .Optimize(Optimize.Minimum)
                .OffspringSelector(new RouletteWheelSelector<DoubleGene, double>())
                .Build();

            var interimResult = engine.Stream()
                .TakeWhile(Limits.BySteadyFitness<DoubleGene, double>(10))
                .ToBestEvolutionResult();

            using (var stream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, interimResult);

                stream.Seek(0, SeekOrigin.Begin);
                var loadedResult = (EvolutionResult<DoubleGene, double>) formatter.Deserialize(stream);

                var result = engine.Stream(loadedResult)
                    .Take(10)
                    .ToBestEvolutionResult();

                Assert.NotEqual(0.0, result.GetBestFitness());
            }
        }
    }
}