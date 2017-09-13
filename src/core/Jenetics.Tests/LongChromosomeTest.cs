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

namespace Jenetics
{
    public class LongChromosomeTest : NumericChromosomeTesterBase<long, LongGene>
    {
        private readonly LongChromosome _factory = new LongChromosome(
            0, long.MaxValue, 500
        );

        protected override Factory<IChromosome<LongGene>> Factory()
        {
            return () => _factory;
        }
    }
}