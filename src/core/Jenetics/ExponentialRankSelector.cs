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
using Jenetics.Internal.Util;

namespace Jenetics
{
    public class ExponentialRankSelector<TGene, TAllele> : ProbabilitySelectorBase<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly double _c;

        public ExponentialRankSelector(double c) : base(true)
        {
            if (c < 0.0 || c >= 1.0)
                throw new ArgumentOutOfRangeException(nameof(c), $"Value {c} is out of range [0..1): ");
            _c = c;
        }

        public ExponentialRankSelector() : this(0.975)
        {
        }

        public override double[] Probabilities(Population<TGene, TAllele> population, int count)
        {
            double n = population.Count;
            var probabilities = new double[population.Count];

            var b = (_c - 1.0) / (Math.Pow(_c, n) - 1.0);
            for (var i = 0; i < probabilities.Length; ++i)
                probabilities[i] = Math.Pow(_c, i) * b;

            return probabilities;
        }

        public override bool Equals(object obj)
        {
            return obj is ExponentialRankSelector<TGene, TAllele> selector &&
                   Comparer<double>.Default.Compare(_c, selector._c) == 0;
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(_c).Value;
        }

        public override string ToString()
        {
            return $"{GetType()}[c={_c}]";
        }
    }
}