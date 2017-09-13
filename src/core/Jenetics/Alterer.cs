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
using Jenetics.Util;

namespace Jenetics
{
    public interface IAlterer<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        int Alter(Population<TGene, TAllele> population, long generation);
    }

    internal class DefaultAlterer<TGene, TAllele> : IAlterer<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        public int Alter(Population<TGene, TAllele> population, long generation)
        {
            return 0;
        }
    }

    public static class Alterer
    {
        public const double DefaultAlterProbability = 0.2;

        public static IAlterer<TGene, TAllele> Of<TGene, TAllele>(params IAlterer<TGene, TAllele>[] alterers)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return alterers.Length == 0
                ? new DefaultAlterer<TGene, TAllele>()
                : alterers.Length == 1
                    ? alterers[0]
                    : alterers.Length == 1
                        ? alterers.First()
                        : new CompositeAlterer<TGene, TAllele>(ImmutableSeq.Of(alterers));
        }

        public static IAlterer<TGene, TAllele> Compose<TGene, TAllele>(this IAlterer<TGene, TAllele> alterer,
            IAlterer<TGene, TAllele> before)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return Of(before, alterer);
        }
    }
}