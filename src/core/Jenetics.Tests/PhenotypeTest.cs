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
using Jenetics.Util;

namespace Jenetics
{
    public class PhenotypeTest : ObjectTesterBase<Phenotype<DoubleGene, double>>
    {
        private readonly Func<Genotype<DoubleGene>, double> _ff = gt => Math.Sin(ToRadians(gt.Gene.Allele));

        private readonly Factory<Genotype<DoubleGene>> _genotype = () => Genotype.Of(
            DoubleChromosome.Of(0, 1, 50),
            DoubleChromosome.Of(0, 1, 500),
            DoubleChromosome.Of(0, 1, 100),
            DoubleChromosome.Of(0, 1, 50)
        );

        protected override Factory<Phenotype<DoubleGene, double>> Factory()
        {
            return () => Phenotype.Of(_genotype(), 0, _ff).Evaluate();
        }

        private static double ToRadians(double angdeg)
        {
            return angdeg / 180.0 * Math.PI;
        }
    }
}