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
using Jenetics.Stat;

namespace Jenetics.Engine
{
    public abstract class EvolutionStatistics<T, TFitnessStatistics>
        where T : IComparable<T>, IConvertible
        where TFitnessStatistics : class
    {
        protected const string Cpattern = "| {0,22} {1,-51}|\n";
        protected const string Spattern = "| {0,27} {1,-46}|\n";

        protected readonly LongMomentStatistics Age = new LongMomentStatistics();

        public DoubleMomentStatistics SelectionDuration { get; } = new DoubleMomentStatistics();

        public DoubleMomentStatistics AlterDuration { get; } = new DoubleMomentStatistics();

        public DoubleMomentStatistics EvaluationDuration { get; } = new DoubleMomentStatistics();

        public DoubleMomentStatistics EvolveDuration { get; } = new DoubleMomentStatistics();

        public IntMomentStatistics Killed { get; } = new IntMomentStatistics();

        public IntMomentStatistics Invalids { get; } = new IntMomentStatistics();

        public IntMomentStatistics Altered { get; } = new IntMomentStatistics();

        public LongMomentStatistics PhenotypeAge => Age;

        public TFitnessStatistics Fitness { get; protected set; }

        public void Accept<TGene>(EvolutionResult<TGene, T> result)
            where TGene : IGene<TGene>
        {
            Accept(result.GetDurations());

            Killed.Accept(result.GetKillCount());
            Invalids.Accept(result.GetInvalidCount());
            Altered.Accept(result.GetAlterCount());

            foreach (var pt in result.GetPopulation())
                Accept(pt, result.GetGeneration());
        }

        private void Accept(EvolutionDurations durations)
        {
            var selection =
                ToSeconds(durations.GetOffspringSelectionDuration()) +
                ToSeconds(durations.GetSurvivorsSelectionDuration());
            var alter =
                ToSeconds(durations.GetOffspringAlterDuration()) +
                ToSeconds(durations.GetOffspringFilterDuration());

            SelectionDuration.Accept(selection);
            AlterDuration.Accept(alter);
            EvaluationDuration
                .Accept(ToSeconds(durations.GetEvaluationDuration()));
            EvolveDuration
                .Accept(ToSeconds(durations.GetEvolveDuration()));
        }

        private static double ToSeconds(TimeSpan duration)
        {
            return duration.TotalSeconds;
        }

        protected virtual void Accept<TGene>(Phenotype<TGene, T> pt, long generation)
            where TGene : IGene<TGene>
        {
            Age.Accept(pt.GetAge(generation));
        }

        public override string ToString()
        {
            return
                "+---------------------------------------------------------------------------+\n" +
                "|  Time statistics                                                          |\n" +
                "+---------------------------------------------------------------------------+\n" +
                string.Format(Cpattern, "Selection:", D(SelectionDuration)) +
                string.Format(Cpattern, "Altering:", D(AlterDuration)) +
                string.Format(Cpattern, "Fitness calculation:", D(EvaluationDuration)) +
                string.Format(Cpattern, "Overall execution:", D(EvolveDuration)) +
                "+---------------------------------------------------------------------------+\n" +
                "|  Evolution statistics                                                     |\n" +
                "+---------------------------------------------------------------------------+\n" +
                string.Format(Cpattern, "Generations:", I(Altered.Count)) +
                string.Format(Cpattern, "Altered:", I(Altered)) +
                string.Format(Cpattern, "Killed:", I(Killed)) +
                string.Format(Cpattern, "Invalids:", I(Invalids));
        }

        private static string D(DoubleMomentStatistics statistics)
        {
            return string.Format("sum={0:F12} s; mean={1:F12} s", statistics.Sum, statistics.Mean);
        }

        private static string I(IntMomentStatistics statistics)
        {
            return string.Format("sum={0}; mean={1:0.000000000}", statistics.Sum, statistics.Mean);
        }

        private static string I(long value)
        {
            return $"{value}";
        }

        protected static string P(LongMomentStatistics statistics)
        {
            return string.Format("max={0}; mean={1:F6}; var={2:F6}", statistics.Max, statistics.Mean,
                statistics.Variance);
        }
    }

    internal class Num<T> : EvolutionStatistics<T, DoubleMomentStatistics>
        where T : IComparable<T>, IConvertible
    {
        internal Num()
        {
            Fitness = new DoubleMomentStatistics();
        }

        protected override void Accept<TGene>(Phenotype<TGene, T> pt, long generation)
        {
            base.Accept(pt, generation);
            Fitness.Accept(pt.GetFitness().ToDouble(null));
        }

        public override string ToString()
        {
            return base.ToString() +
                   "+---------------------------------------------------------------------------+\n" +
                   "|  Population statistics                                                    |\n" +
                   "+---------------------------------------------------------------------------+\n" +
                   string.Format(Cpattern, "Age:", P(Age)) +
                   string.Format(Cpattern, "Fitness:", "") +
                   string.Format(Spattern, "min  =", D(Fitness.Min)) +
                   string.Format(Spattern, "max  =", D(Fitness.Max)) +
                   string.Format(Spattern, "mean =", D(Fitness.Mean)) +
                   string.Format(Spattern, "var  =", D(Fitness.Variance)) +
                   string.Format(Spattern, "std  =", D(Math.Sqrt(Fitness.Variance))) +
                   "+---------------------------------------------------------------------------+";
        }

        private static string D(double value)
        {
            return string.Format("{0:F12}", value);
        }
    }

    public static class EvolutionStatistics
    {
        public static EvolutionStatistics<T, DoubleMomentStatistics> OfNumber<T>()
            where T : IComparable<T>, IConvertible
        {
            return new Num<T>();
        }
    }
}