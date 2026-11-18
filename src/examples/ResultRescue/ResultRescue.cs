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
using System.IO;
using System.Linq;
using System.Text.Json;
using Jenetics.Engine;
using Jenetics.Util;
using JeneticsEngine = Jenetics.Engine.Engine;
using ProblemFactory = Jenetics.Engine.Problem;

namespace Jenetics.Example;
public static class ResultRescue
{
    private static readonly DoubleRange Range = DoubleRange.Of(0.0, 2.0 * Math.PI);

    private static readonly IProblem<double, DoubleGene, double> Problem = ProblemFactory.Of(
        x => Math.Cos(0.5 + Math.Sin(x)) * Math.Cos(x),
        Codecs.OfScalar(Range)
    );

    private static readonly Engine<DoubleGene, double> Engine = JeneticsEngine.Builder(Problem)
        .Optimize(Optimize.Minimum)
        .OffspringSelector(new RouletteWheelSelector<DoubleGene, double>())
        .Build();

    private sealed record ResultSnapshot(long Generation, double[] Individuals);

    public static void Main()
    {
        var rescue = Engine.Stream()
            .TakeWhile(Limits.BySteadyFitness<DoubleGene, double>(10))
            .ToBestEvolutionResult();

        var snapshot = new ResultSnapshot(
            rescue.GetTotalGenerations(),
            rescue.GetPopulation()
                .Select(phenotype => phenotype.GetGenotype().Gene.DoubleValue())
                .ToArray()
        );
        File.WriteAllText("result.json", JsonSerializer.Serialize(snapshot));

        var restored = JsonSerializer.Deserialize<ResultSnapshot>(File.ReadAllText("result.json"))
            ?? throw new InvalidDataException("The saved evolution result is empty.");

        if (restored.Individuals is null || restored.Individuals.Length == 0)
            throw new InvalidDataException("The saved evolution result contains no individuals.");

        var fitness = Problem.Fitness();
        var decoder = Problem.Codec().Decoder();
        var population = new Population<DoubleGene, double>(restored.Individuals.Length);
        foreach (var value in restored.Individuals)
        {
            var genotype = Genotype.Of(
                DoubleChromosome.Of(DoubleGene.Of(value, Range.Min, Range.Max))
            );
            population.Add(Phenotype.Of(
                genotype,
                restored.Generation,
                individual => fitness(decoder(individual))
            ));
        }

        var result = Engine
            .Stream(population, restored.Generation)
            .TakeWhile(Limits.BySteadyFitness<DoubleGene, double>(20))
            .ToBestEvolutionResult();

        Console.WriteLine(result);
    }
}