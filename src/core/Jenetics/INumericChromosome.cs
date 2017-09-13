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

namespace Jenetics
{
    public interface INumericChromosome<out TAllele, TGene> : IBoundedChromosome<TAllele, TGene>
        where TAllele : IComparable<TAllele>, IConvertible
        where TGene : INumericGene<TAllele, TGene>
    {
    }

    public static class NumericChromosomeExtensions
    {
        public static byte ByteValue<TAllele, TGene>(this INumericChromosome<TAllele, TGene> nc, int index)
            where TAllele : IComparable<TAllele>, IConvertible
            where TGene : INumericGene<TAllele, TGene>
        {
            return nc.GetGene(index).Allele.ToByte(null);
        }

        public static byte ByteValue<TAllele, TGene>(this INumericChromosome<TAllele, TGene> nc)
            where TAllele : IComparable<TAllele>, IConvertible
            where TGene : INumericGene<TAllele, TGene>
        {
            return nc.ByteValue(0);
        }

        public static short ShortValue<TAllele, TGene>(this INumericChromosome<TAllele, TGene> nc, int index)
            where TAllele : IComparable<TAllele>, IConvertible
            where TGene : INumericGene<TAllele, TGene>
        {
            return nc.GetGene(index).Allele.ToInt16(null);
        }

        public static short ShortValue<TAllele, TGene>(this INumericChromosome<TAllele, TGene> nc)
            where TAllele : IComparable<TAllele>, IConvertible
            where TGene : INumericGene<TAllele, TGene>
        {
            return nc.ShortValue(0);
        }

        public static int IntValue<TAllele, TGene>(this INumericChromosome<TAllele, TGene> nc, int index)
            where TAllele : IComparable<TAllele>, IConvertible
            where TGene : INumericGene<TAllele, TGene>
        {
            return nc.GetGene(index).Allele.ToInt32(null);
        }

        public static int IntValue<TAllele, TGene>(this INumericChromosome<TAllele, TGene> nc)
            where TAllele : IComparable<TAllele>, IConvertible
            where TGene : INumericGene<TAllele, TGene>
        {
            return nc.IntValue(0);
        }

        public static float FloatValue<TAllele, TGene>(this INumericChromosome<TAllele, TGene> nc, int index)
            where TAllele : IComparable<TAllele>, IConvertible
            where TGene : INumericGene<TAllele, TGene>
        {
            return nc.GetGene(index).Allele.ToSingle(null);
        }

        public static float FloatValue<TAllele, TGene>(this INumericChromosome<TAllele, TGene> nc)
            where TAllele : IComparable<TAllele>, IConvertible
            where TGene : INumericGene<TAllele, TGene>
        {
            return nc.FloatValue(0);
        }

        public static double DoubleValue<TAllele, TGene>(this INumericChromosome<TAllele, TGene> nc, int index)
            where TAllele : IComparable<TAllele>, IConvertible
            where TGene : INumericGene<TAllele, TGene>
        {
            return nc.GetGene(index).Allele.ToDouble(null);
        }

        public static double DoubleValue<TAllele, TGene>(this INumericChromosome<TAllele, TGene> nc)
            where TAllele : IComparable<TAllele>, IConvertible
            where TGene : INumericGene<TAllele, TGene>
        {
            return nc.DoubleValue(0);
        }
    }
}