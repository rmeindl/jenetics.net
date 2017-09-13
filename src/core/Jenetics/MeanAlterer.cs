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
    public class MeanAlterer<TGene, TAllele> : RecombinatorBase<TGene, TAllele>
        where TGene : IGene<TGene>, IMean<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        public MeanAlterer(double probability) : base(probability, 2)
        {
        }

        public MeanAlterer() : this(0.05)
        {
        }

        protected internal override int Recombine(Population<TGene, TAllele> population, int[] individuals,
            long generation)
        {
            var random = RandomRegistry.GetRandom();

            var pt1 = population[individuals[0]];
            var pt2 = population[individuals[1]];
            var gt1 = pt1.GetGenotype();
            var gt2 = pt2.GetGenotype();

            var cindex = random.NextInt(Math.Min(gt1.Length, gt2.Length));

            var c1 = gt1.ToSeq().Copy();
            var c2 = gt2.ToSeq();

            var mean = Mean(
                c1[cindex].ToSeq().Copy(),
                c2[cindex].ToSeq()
            );

            c1[cindex] = c1[cindex].NewInstance(mean.ToImmutableSeq());

            population[individuals[0]] = pt1.NewInstance(gt1.NewInstance(c1.ToImmutableSeq()), generation);

            return 1;
        }

        private static IMutableSeq<TGene> Mean(IMutableSeq<TGene> a, ISeq<TGene> b)
        {
            for (var i = a.Length; --i >= 0;)
                a[i] = a[i].Mean(b[i]);
            return a;
        }
    }
}