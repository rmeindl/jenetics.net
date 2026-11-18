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
using System.Linq;
using Jenetics.Engine;
using Jenetics.Util;
using JeneticsEngine = Jenetics.Engine.Engine;

namespace Jenetics.Example;
public static class MpLStrategy
{
    private static double Fitness(double x)
    {
        return x;
    }

    public static void Main()
    {
        const int mu = 5;
        const int lambda = 20;
        const double mutationProbability = 0.2;

        var codec = Codecs.OfScalar(DoubleRange.Of(0, 1));

        var engine = JeneticsEngine
            .Builder(Fitness, codec)
            .PopulationSize(lambda)
            .SurvivorsSize(mu)
            .Selector(new TruncationSelector<DoubleGene, double>(mu))
            .Alterers(new Mutator<DoubleGene, double>(mutationProbability))
            .Build();

        Console.WriteLine(
            codec.Decode(
                engine.Stream()
                    .Take(100)
                    .ToBestGenotype()
            )
        );
    }
}