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
    /// <summary>
    ///     Genes are the atoms of the <em>Jenetics</em> library. They contain the actual
    ///     information(alleles) of the encoded solution.All implementations of the
    ///     this interface are final, immutable and can be only created via static
    ///     factory methods which have the name <c>Of</c>. When extending the library
    ///     with own <c>IGene</c> implementations, it is recommended to also implement it
    ///     as <a href="https://en.wikipedia.org/wiki/Value_object"> value objects</a>.
    /// </summary>
    /// <typeparam name="TAllele">the <a href="http://en.wikipedia.org/wiki/Allele">Allele</a> type of this gene.</typeparam>
    /// <typeparam name="TGene"></typeparam>
    public interface IGene<TAllele, out TGene> : IVerifiable, IGene<TGene>
        where TGene : IGene<TAllele, TGene>
    {
        /// <summary>
        ///     Return the allele of this gene.
        /// </summary>
        TAllele Allele { get; }

        /// <summary>
        ///     Create a new gene from the given <c>value</c> and the gene context.
        /// </summary>
        /// <param name="value">the value of the new gene.</param>
        /// <returns>a new gene with the given value.</returns>
        TGene NewInstance(TAllele value);
    }

    public interface IGene<out TGene> : IFactory<TGene>
        where TGene : IGene<TGene>
    {
    }
}