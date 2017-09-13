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

namespace Jenetics.Example
{
    public static class HelloWorld
    {
        // 2.) Definition of the fitness function.
        private static int Eval(Genotype<BitGene> gt)
        {
            return gt.GetChromosome().As<BitChromosome>().BitCount();
        }

        public static void Main()
        {
            // 1.) Define the genotype (factory) suitable
            //     for the problem.
            Genotype<BitGene> F()
            {
                return Genotype.Of(BitChromosome.Of(10, 0.5));
            }

            // 3.) Create the execution environment.
            var engine = Engine.Engine.Builder(Eval, F).Build();

            // 4.) Start the execution (evolution) and
            //     collect the result.
            var result = engine.Stream().Take(100).ToBestGenotype();

            Console.WriteLine("Hello World:\n" + result);
        }
    }
}