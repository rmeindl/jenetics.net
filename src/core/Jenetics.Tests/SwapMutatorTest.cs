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

using System.Collections.Generic;
using Xunit;

namespace Jenetics
{
    public class SwapMutatorTest : MutatorTesterBase
    {
        protected override IAlterer<DoubleGene, double> NewAlterer(double p)
        {
            return new SwapMutator<DoubleGene, double>(p);
        }

        [Theory]
        [ClassData(typeof(TestUtils.AlterCountParameters))]
        public override void AlterCount(int ngenes, int nchromosomes, int npopulation)
        {
            var p1 = TestUtils.NewDoubleGenePopulation(ngenes, nchromosomes, npopulation);
            var p2 = p1.Copy();
            Assert.Equal(p1, p2);

            var mutator = NewAlterer(0.01);

            var alterations = mutator.Alter(p1, 1);
            //var diff = TestUtils.Diff(p1, p2);

            if (ngenes == 1)
            {
                Assert.Equal(0, alterations);
            }
            else 
            {
                //Assert.True(alterations >= diff/2, $"{alterations} >= {diff/2}");
                //Assert.True(alterations <= 2*diff, $"{alterations} < {2 * diff}");
            }
        }

        [Theory]
        [MemberData(nameof(AlterProbabilityParameters))]
        public override void AlterProbability(int ngenes, int nchromosomes, int npopulation, double p)
        {
            base.AlterProbability(ngenes, nchromosomes, npopulation, p);
        }

        public static IEnumerable<object[]> AlterProbabilityParameters()
        {
            return new List<object[]>
            {
                new object[] {180, 1, 150, 0.15},
                new object[] {180, 2, 150, 0.15},
                new object[] {180, 15, 150, 0.15},

                new object[] {180, 1, 150, 0.5},
                new object[] {180, 2, 150, 0.5},
                new object[] {180, 15, 150, 0.5},

                new object[] {180, 1, 150, 0.85},
                new object[] {180, 2, 150, 0.85},
                new object[] {180, 15, 150, 0.85}
            };
        }
    }
}