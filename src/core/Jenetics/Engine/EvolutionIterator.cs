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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Jenetics.Engine
{
    public class EvolutionIterator<TGene, TAllele> : IEnumerator<EvolutionResult<TGene, TAllele>>,
        IEnumerable<EvolutionResult<TGene, TAllele>>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly CancellationToken _cancellationToken;

        private readonly Func<EvolutionStart<TGene, TAllele>, CancellationToken, EvolutionResult<TGene, TAllele>>
            _evolution;

        private readonly Func<EvolutionStart<TGene, TAllele>> _initial;

        private EvolutionStart<TGene, TAllele> _start;

        public EvolutionIterator(
            Func<EvolutionStart<TGene, TAllele>> initial,
            Func<EvolutionStart<TGene, TAllele>, CancellationToken, EvolutionResult<TGene, TAllele>> evolution,
            CancellationToken cancellationToken
        )
        {
            _initial = initial;
            _evolution = evolution;
            _cancellationToken = cancellationToken;
        }

        public IEnumerator<EvolutionResult<TGene, TAllele>> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public EvolutionResult<TGene, TAllele> Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_start == null)
                _start = _initial();

            Current = _evolution(_start, _cancellationToken);
            _start = Current.Next();

            return true;
        }

        public void Reset()
        {
        }
    }
}