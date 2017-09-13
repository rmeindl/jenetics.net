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

namespace Jenetics.Engine
{
    public interface IProblem<T, TGene, out TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        Func<T, TAllele> Fitness();

        ICodec<T, TGene> Codec();
    }

    public static class Problem
    {
        public static IProblem<T, TGene, TAllele> Of<T, TGene, TAllele>(Func<T, TAllele> fitness,
            ICodec<T, TGene> codec)
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>
        {
            return new DefaultProblem<T, TGene, TAllele>(fitness, codec);
        }
    }

    internal class DefaultProblem<T, TGene, TAllele> : IProblem<T, TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        private readonly ICodec<T, TGene> _codec;
        private readonly Func<T, TAllele> _fitness;

        public DefaultProblem(Func<T, TAllele> fitness, ICodec<T, TGene> codec)
        {
            _fitness = fitness;
            _codec = codec;
        }

        public Func<T, TAllele> Fitness()
        {
            return _fitness;
        }

        public ICodec<T, TGene> Codec()
        {
            return _codec;
        }
    }
}