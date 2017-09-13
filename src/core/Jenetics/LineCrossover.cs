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
using Jenetics.Util;

namespace Jenetics
{
    public class LineCrossover<TGene, TAllele> : CrossoverBase<TGene, TAllele>
        where TGene : INumericGene<TAllele, TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly double _p;

        public LineCrossover(double probability, double p = 0) : base(probability)
        {
            _p = Require.NonNegative(p, "p");
        }

        public LineCrossover() : this(Alterer.DefaultAlterProbability)
        {
        }

        protected internal override int Crossover(IMutableSeq<TGene> v, IMutableSeq<TGene> w)
        {
            var random = RandomRegistry.GetRandom();

            var min = v[0].Min.ToDouble(null);
            var max = v[0].Max.ToDouble(null);

            var a = Internal.Math.random.NextDouble(random, -_p, 1 + _p);
            var b = Internal.Math.random.NextDouble(random, -_p, 1 + _p);

            var changed = false;
            for (int i = 0, n = Math.Min(v.Length, w.Length); i < n; ++i)
            {
                var vi = v[i].DoubleValue();
                var wi = w[i].DoubleValue();

                dynamic t = a * vi + (1 - a) * wi;
                dynamic s = b * wi + (1 - b) * vi;

                if (t >= min && s >= min && t < max && s < max)
                {
                    v[i] = v[i].NewInstance(t);
                    w[i] = w[i].NewInstance(s);
                    changed = true;
                }
            }

            return changed ? 2 : 0;
        }

        public override bool Equals(object obj)
        {
            return obj is LineCrossover<TGene, TAllele> && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(base.GetHashCode()).Value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}[p={Probability}]";
        }
    }
}