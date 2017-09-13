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
using Jenetics.Internal.Util;

namespace Jenetics
{
    [Serializable]
    public class Population<TGene, TAllele> : IList<Phenotype<TGene, TAllele>>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        private readonly List<Phenotype<TGene, TAllele>> _population;

        private Population(List<Phenotype<TGene, TAllele>> population)
        {
            _population = population;
            IsReadOnly = false;
        }

        public Population(int size) : this(new List<Phenotype<TGene, TAllele>>(size))
        {
        }

        public Population() : this(0)
        {
        }

        public bool IsEmpty => _population.Count == 0;

        public IEnumerator<Phenotype<TGene, TAllele>> GetEnumerator()
        {
            return _population.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Phenotype<TGene, TAllele> item)
        {
            _population.Add(item);
        }

        public void Clear()
        {
            _population.Clear();
        }

        public bool Contains(Phenotype<TGene, TAllele> item)
        {
            return _population.Contains(item);
        }

        public void CopyTo(Phenotype<TGene, TAllele>[] array, int arrayIndex)
        {
            _population.CopyTo(array, arrayIndex);
        }

        public bool Remove(Phenotype<TGene, TAllele> item)
        {
            return _population.Remove(item);
        }

        public int Count => _population.Count;

        public bool IsReadOnly { get; }

        public int IndexOf(Phenotype<TGene, TAllele> item)
        {
            return _population.IndexOf(item);
        }

        public void Insert(int index, Phenotype<TGene, TAllele> item)
        {
            _population.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _population.RemoveAt(index);
        }

        public Phenotype<TGene, TAllele> this[int index]
        {
            get => _population[index];
            set => _population[index] = value;
        }

        public Population<TGene, TAllele> Fill(Func<Phenotype<TGene, TAllele>> factory, int count)
        {
            for (var i = 0; i < count; i++)
                _population.Add(factory());
            return this;
        }

        public Population<TGene, TAllele> Copy()
        {
            return new Population<TGene, TAllele>(new List<Phenotype<TGene, TAllele>>(_population));
        }

        public void PopulationSort()
        {
            SortWith(Optimize.Maximum.Descending<TAllele>());
        }

        public void SortWith(IComparer<TAllele> comparator)
        {
            _population.Sort((x, y) =>  comparator.Compare(x.GetFitness(), y.GetFitness()));
        }

        public bool AddAll(IEnumerable<Phenotype<TGene, TAllele>> c)
        {
            var wasChanged = false;
            foreach (var item in c)
            {
                Add(item);
                wasChanged = true;
            }
            return wasChanged;
        }

        public IList<Phenotype<TGene, TAllele>> SubList(int fromIndex, int toIndex)
        {
            IList<Phenotype<TGene, TAllele>> subList = new List<Phenotype<TGene, TAllele>>();
            for (var i = fromIndex; i < fromIndex + toIndex; i++)
                subList.Add(_population[i]);
            return subList;
        }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj)(p => Equality.Eq(_population, p._population));
        }

        public override int GetHashCode()
        {
            return Hash.Of(GetType()).And(_population).Value;
        }

        public override string ToString()
        {
            return string.Join("\n", _population) + "\n";
        }
    }

    public static class Population
    {
        public static Population<TGene, TAllele> Empty<TGene, TAllele>()
            where TGene : IGene<TGene>
            where TAllele : IComparable<TAllele>, IConvertible
        {
            return new Population<TGene, TAllele>(0);
        }
    }
}