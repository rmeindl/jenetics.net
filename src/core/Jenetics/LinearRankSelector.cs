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
using Jenetics.Internal.Util;

namespace Jenetics
{
    public class LinearRankSelector<TGene, TAllele> : ProbabilitySelectorBase<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly double _nminus;
        private readonly double _nplus;

        public LinearRankSelector(double nminus) : base(true)
        {
            if (nminus < 0)
                throw new ArgumentOutOfRangeException(nameof(nminus), $"nminus is smaller than zero: {nminus}");

            _nminus = nminus;
            _nplus = 2 - _nminus;
        }

        public LinearRankSelector() : this(0.5)
        {
        }

        public override double[] Probabilities(Population<TGene, TAllele> population, int count)
        {
            double n = population.Count;
            var probabilities = new double[population.Count];

            if (n.Equals(1))
                probabilities[0] = 1;
            else
                for (var i = probabilities.Length; --i >= 0;)
                    probabilities[probabilities.Length - i - 1] =
                        (_nminus + (_nplus - _nminus) * i / (n - 1)) / n;

            return probabilities;
        }

        public override bool Equals(object obj)
        {
            return obj is LinearRankSelector<TGene, TAllele> selector &&
                   ProbabilitySelector.Eq(selector._nminus, _nminus) &&
                   ProbabilitySelector.Eq(selector._nplus, _nplus);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(_nminus).And(_nplus).Value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}[(n-)={_nminus}, (n+)={_nplus}]";
        }
    }
}