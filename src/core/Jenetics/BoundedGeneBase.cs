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

namespace Jenetics
{
    public interface IBoundedGene<TAllele, TGene> : IGene<TAllele, TGene>, IComparable<TGene>
        where TAllele : IComparable<TAllele>
        where TGene : IBoundedGene<TAllele, TGene>
    {
        TAllele Min { get; }
        TAllele Max { get; }
    }

    [Serializable]
    public abstract class BoundedGeneBase<TAllele, TGene> : IBoundedGene<TAllele, TGene>
        where TAllele : IComparable<TAllele>
        where TGene : BoundedGeneBase<TAllele, TGene>
    {
        private readonly bool _valid;
        internal readonly TAllele Value;

        protected BoundedGeneBase(TAllele value, TAllele min, TAllele max)
        {
            Min = Require.NonNull(min);
            Max = Require.NonNull(max);
            Value = Require.NonNull(value);
            _valid = Value.CompareTo(min) >= 0 && Value.CompareTo(max) <= 0;
        }

        public abstract int CompareTo(TGene other);
        public abstract TGene NewInstance();
        public abstract TGene NewInstance(TAllele value);

        public TAllele Allele => Value;

        public bool IsValid => _valid;

        public TAllele Min { get; }
        public TAllele Max { get; }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(gene =>
                Equality.Eq(Value, gene.Value) &&
                Equality.Eq(Min, gene.Min) &&
                Equality.Eq(Max, gene.Max)
            );
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType())
                .And(Value)
                .And(Min)
                .And(Max).Value;
        }

        public override string ToString()
        {
            return $"[{Value}]";
        }
    }
}