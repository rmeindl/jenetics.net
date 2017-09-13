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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Jenetics.Engine;
using Jenetics.Util;

namespace Jenetics.Example
{
    public static class ResultRescue
    {
        private static readonly IProblem<double, DoubleGene, double> Problem = Jenetics.Engine.Problem.Of(
            x => Math.Cos(0.5 + Math.Sin(x)) * Math.Cos(x),
            Codecs.OfScalar(DoubleRange.Of(0.0, 2.0 * Math.PI))
        );

        private static readonly Engine<DoubleGene, double> Engine = Jenetics.Engine.Engine.Builder(Problem)
            .Optimize(Optimize.Minimum)
            .OffspringSelector(new RouletteWheelSelector<DoubleGene, double>())
            .Build();

        public static void Main()
        {
            var rescue = Engine.Stream()
                .TakeWhile(Limits.BySteadyFitness<DoubleGene, double>(10))
                .ToBestEvolutionResult();

            using (var stream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, rescue);
                File.WriteAllBytes("result.bin", stream.ToArray());
            }

            using (var stream = File.OpenRead("result.bin"))
            {
                IFormatter formatter = new BinaryFormatter();
                var result = Engine
                    .Stream((EvolutionResult<DoubleGene, double>) formatter.Deserialize(stream))
                    .TakeWhile(Limits.BySteadyFitness<DoubleGene, double>(20))
                    .ToBestEvolutionResult();

                Console.WriteLine(result);
            }
        }
    }
}