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
using Jenetics.Internal.Math;
using Jenetics.Internal.Util;
using Jenetics.Util;

namespace Jenetics
{
    public abstract class ProbabilitySelectorBase<TGene, TAllele> : ISelector<TGene, TAllele>
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>, IConvertible
    {
        private readonly Func<double[], double[]> _reverter;
        private readonly bool _sorted;

        protected ProbabilitySelectorBase() : this(false)
        {
        }

        protected ProbabilitySelectorBase(bool sorted)
        {
            _sorted = sorted;
            if (sorted)
                _reverter = Arrays.Revert;
            else
                _reverter = ProbabilitySelector.SortAndRevert;
        }

        public virtual Population<TGene, TAllele> Select(Population<TGene, TAllele> population, int count, Optimize opt)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count),
                    $"Selection count must be greater or equal then zero, but was {count}.");

            var selection = new Population<TGene, TAllele>(count);
            if (count > 0 && !population.IsEmpty)
            {
                var pop = Copy(population);

                var prob = Probabilities(pop, count, opt);

                CheckAndCorrect(prob);

                ProbabilitySelector.Incremental(prob);

                var random = RandomRegistry.GetRandom();
                selection.Fill(() => pop[ProbabilitySelector.IndexOf(prob, random.NextDouble())], count);
            }

            return selection;
        }

        public abstract double[] Probabilities(Population<TGene, TAllele> population, int count);

        public double[] Probabilities(Population<TGene, TAllele> population, int count, Optimize opt)
        {
            return opt == Optimize.Minimum
                ? _reverter(Probabilities(population, count))
                : Probabilities(population, count);
        }

        protected Population<TGene, TAllele> Copy(Population<TGene, TAllele> population)
        {
            var pop = population;
            if (!_sorted) return pop;

            pop = population.Copy();
            pop.PopulationSort();

            return pop;
        }

        private static void CheckAndCorrect(double[] probabilities)
        {
            var ok = true;
            for (var i = probabilities.Length; --i >= 0 && ok;)
                ok = !double.IsInfinity(probabilities[i]);

            if (ok) return;

            var value = 1.0 / probabilities.Length;
            for (var i = probabilities.Length; --i >= 0;)
                probabilities[i] = value;
        }
    }

    public static class ProbabilitySelector
    {
        private const int SerialIndexThreshold = 35;
        private static readonly long MaxUlpDistance = Arithmetics.Pow(10, 10);

        public static double[] Incremental(double[] values)
        {
            var adder = new DoubleAdder(values[0]);
            for (var i = 1; i < values.Length; ++i)
                values[i] = adder.Add(values[i]).DoubleValue;
            return values;
        }

        public static int IndexOf(double[] incr, double v)
        {
            return incr.Length <= SerialIndexThreshold
                ? IndexOfSerial(incr, v)
                : IndexOfBinary(incr, v);
        }

        public static bool Sum2One(double[] probabilities)
        {
            var sum = probabilities.Length > 0
                ? DoubleAdder.Sum(probabilities)
                : 1.0;
            return Math.Abs(Base.UlpDistance(sum, 1.0)) < MaxUlpDistance;
        }

        public static bool Eq(double a, double b)
        {
            return Math.Abs(Base.UlpDistance(a, b)) < MaxUlpDistance;
        }

        internal static double[] SortAndRevert(double[] array)
        {
            var indexes = IndexSorter.Sort(array);

            var result = new double[array.Length];
            for (var i = 0; i < result.Length; ++i)
                result[indexes[result.Length - 1 - i]] = array[indexes[i]];

            return result;
        }

        internal static int IndexOfSerial(double[] incr, double v)
        {
            var index = -1;
            for (var i = 0; i < incr.Length && index == -1; ++i)
                if (incr[i] >= v)
                    index = i;

            return index;
        }

        internal static int IndexOfBinary(double[] incr, double v)
        {
            var imin = 0;
            var imax = incr.Length;
            var index = -1;

            while (imax > imin && index == -1)
            {
                var imid = Bits.bit_rol(imin + imax, 1);

                if (imid == 0 || incr[imid] >= v && incr[imid - 1] < v)
                    index = imid;
                else if (incr[imid] <= v)
                    imin = imid + 1;
                else if (incr[imid] > v)
                    imax = imid;
            }

            return index;
        }
    }
}