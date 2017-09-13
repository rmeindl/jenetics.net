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
using Jenetics.Util;

namespace Jenetics
{
    /// <summary>
    ///     Implementation of a BitGene.
    /// </summary>
    public class BitGene : IGene<bool, BitGene>, IComparable<BitGene>
    {
        public static readonly BitGene True = new BitGene(true);
        public static readonly BitGene False = new BitGene(false);
        public static readonly BitGene Zero = False;
        public static readonly BitGene One = True;

        private BitGene(bool value)
        {
            Allele = value;
        }

        /// <inheritdoc />
        public int CompareTo(BitGene other)
        {
            return Comparer<bool>.Default.Compare(Allele, other.Allele);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Return alwas <c>true.</c>
        /// </summary>
        /// <returns>always <c>true</c>.</returns>
        public bool IsValid => true;

        /// <inheritdoc />
        /// <summary>
        ///     Create a new, <em>random</em> gene.
        /// </summary>
        public BitGene NewInstance()
        {
            return RandomRegistry.GetRandom().Next(1) == 1 ? True : False;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Create a new gene from the given <c>value</c>.
        /// </summary>
        public BitGene NewInstance(bool value)
        {
            return Of(value);
        }

        /// <inheritdoc />
        public bool Allele { get; }

        /// <summary>
        ///     Return the <c>boolean</c> value of this gene.
        /// </summary>
        /// <see cref="Allele" />
        /// <returns>the <c>boolean</c> value of this gene.></returns>
        public bool BooleanValue()
        {
            return Allele;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Allele}";
        }

        /// <summary>
        ///     Return the corresponding <c>BitGene</c> for the given <c>boolean</c> value.
        /// </summary>
        /// <param name="value">the value of the returned <c>BitGene</c>.</param>
        /// <returns>the <c>BitGene</c> for the given <c>boolean</c> value.</returns>
        public static BitGene Of(bool value)
        {
            return value ? True : False;
        }

        /// <summary>
        ///     Return the value of the BitGene.
        /// </summary>
        /// <returns>The value of the BitGene.</returns>
        public bool GetBit()
        {
            return Allele;
        }
    }
}