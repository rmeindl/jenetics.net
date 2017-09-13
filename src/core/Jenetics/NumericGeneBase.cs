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
    public interface INumericGene<TAllele, TGene> : IBoundedGene<TAllele, TGene>
        where TAllele : IComparable<TAllele>, IConvertible
        where TGene : INumericGene<TAllele, TGene>
    {
        int IntValue();
        long LongValue();
        float FloatValue();
        double DoubleValue();
    }

    [Serializable]
    public abstract class NumericGeneBase<TAllele, TGene> : BoundedGeneBase<TAllele, TGene>,
        INumericGene<TAllele, TGene>
        where TAllele : IComparable<TAllele>, IConvertible
        where TGene : NumericGeneBase<TAllele, TGene>
    {
        public NumericGeneBase(TAllele value, TAllele min, TAllele max) : base(value, min, max)
        {
        }

        public int IntValue()
        {
            return Allele.ToInt32(null);
        }

        public long LongValue()
        {
            return Allele.ToInt64(null);
        }

        public float FloatValue()
        {
            return (float) DoubleValue();
        }

        public double DoubleValue()
        {
            return Allele.ToDouble(null);
        }
    }
}