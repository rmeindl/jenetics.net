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

using System.Collections;
using System.Collections.Generic;
using Xunit;
using static Jenetics.TestUtils;

namespace Jenetics
{
    public abstract class AltererTesterBase
    {
        protected abstract IAlterer<DoubleGene, double> NewAlterer(double p);

        [Theory]
        [ClassData(typeof(ParametersDataGenerator))]
        public void AlterParameters(
            int ngenes,
            int nchromosomes,
            int npopulation
        )
        {
            var population = NewDoubleGenePopulation(
                ngenes, nchromosomes, npopulation
            );

            var alterer = NewAlterer(1);

            alterer.Alter(population, 1);
        }

        private class ParametersDataGenerator : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] {20, 20, 0},
                new object[] {1, 1, 0},
                new object[] {5, 1, 0},
                new object[] {80, 1, 0},
                new object[] {20, 20, 1},
                new object[] {1, 1, 1},
                new object[] {5, 1, 1},
                new object[] {80, 1, 1},
                new object[] {20, 20, 2},
                new object[] {1, 1, 2},
                new object[] {5, 1, 2},
                new object[] {80, 1, 2},
                new object[] {20, 20, 3},
                new object[] {1, 1, 3},
                new object[] {5, 1, 3},
                new object[] {80, 1, 3}
            };

            public IEnumerator<object[]> GetEnumerator()
            {
                return _data.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}