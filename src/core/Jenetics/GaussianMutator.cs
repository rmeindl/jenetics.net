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
using Jenetics.Internal.Math;
using Jenetics.Util;

namespace Jenetics
{
    public class GaussianMutator<TGene, TAllele> : Mutator<TGene, TAllele>
        where TGene : INumericGene<TAllele, TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private bool _haveNextNextGaussian;

        private double _nextNextGaussian;

        public GaussianMutator(double probability) : base(probability)
        {
        }

        public GaussianMutator() : this(Alterer.DefaultAlterProbability)
        {
        }

        protected override int Mutate(IMutableSeq<TGene> genes, double p)
        {
            var random = RandomRegistry.GetRandom();

            return Internal.Math.random.Indexes(random, genes.Length, p)
                .Peek(i => genes[i] = Mutate(genes[i], random))
                .Count();
        }

        private TGene Mutate(TGene gene, Random random)
        {
            var min = gene.Min.ToDouble(null);
            var max = gene.Max.ToDouble(null);
            var std = (max - min) * 0.25;

            var value = gene.DoubleValue();
            var gaussian = NextGaussian(random);

            dynamic clamped = Base.Clamp(gaussian * std + value, min, max);
            return gene.NewInstance(clamped);
        }

        private double NextGaussian(Random random)
        {
            // See Knuth, ACP, Section 3.4.1 Algorithm C.
            if (_haveNextNextGaussian)
            {
                _haveNextNextGaussian = false;
                return _nextNextGaussian;
            }
            double v1, v2, s;
            do
            {
                v1 = 2 * random.NextDouble() - 1; // between -1 and 1
                v2 = 2 * random.NextDouble() - 1; // between -1 and 1
                s = v1 * v1 + v2 * v2;
            } while (s >= 1 || s.Equals(0.0));
            var multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
            _nextNextGaussian = v2 * multiplier;
            _haveNextNextGaussian = true;
            return v1 * multiplier;
        }
    }
}