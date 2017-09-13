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
    [Serializable]
    public class EvolutionDurations
    {
        public static readonly EvolutionDurations Zero = Of(
            TimeSpan.Zero,
            TimeSpan.Zero,
            TimeSpan.Zero,
            TimeSpan.Zero,
            TimeSpan.Zero,
            TimeSpan.Zero,
            TimeSpan.Zero
        );

        private readonly TimeSpan _evolveDuration;
        private readonly TimeSpan _evaluationDuration;
        private readonly TimeSpan _offspringAlterDuration;
        private readonly TimeSpan _offspringFilterDuration;

        private readonly TimeSpan _offspringSelectionDuration;
        private readonly TimeSpan _survivorFilterDuration;
        private readonly TimeSpan _survivorsSelectionDuration;

        private EvolutionDurations(
            TimeSpan offspringSelectionDuration,
            TimeSpan survivorsSelectionDuration,
            TimeSpan offspringAlterDuration,
            TimeSpan offspringFilterDuration,
            TimeSpan survivorFilterDuration,
            TimeSpan evaluationDuration,
            TimeSpan evolveDuration
        )
        {
            _offspringSelectionDuration = offspringSelectionDuration;
            _survivorsSelectionDuration = survivorsSelectionDuration;
            _offspringAlterDuration = offspringAlterDuration;
            _offspringFilterDuration = offspringFilterDuration;
            _survivorFilterDuration = survivorFilterDuration;
            _evaluationDuration = evaluationDuration;
            _evolveDuration = evolveDuration;
        }

        public TimeSpan GetEvolveDuration()
        {
            return _evolveDuration;
        }

        public TimeSpan GetOffspringSelectionDuration()
        {
            return _offspringSelectionDuration;
        }

        public TimeSpan GetSurvivorsSelectionDuration()
        {
            return _survivorsSelectionDuration;
        }

        public TimeSpan GetOffspringAlterDuration()
        {
            return _offspringAlterDuration;
        }

        public TimeSpan GetOffspringFilterDuration()
        {
            return _offspringFilterDuration;
        }

        public TimeSpan GetEvaluationDuration()
        {
            return _evaluationDuration;
        }

        public override bool Equals(object obj)
        {
            return obj is EvolutionDurations durations &&
                   Equals(_offspringSelectionDuration,
                       durations._offspringSelectionDuration) &&
                   Equals(_survivorsSelectionDuration,
                       durations._survivorsSelectionDuration) &&
                   Equals(_offspringAlterDuration,
                       durations._offspringAlterDuration) &&
                   Equals(_offspringFilterDuration,
                       durations._offspringFilterDuration) &&
                   Equals(_survivorFilterDuration,
                       durations._survivorFilterDuration) &&
                   Equals(_evaluationDuration,
                       durations._evaluationDuration) &&
                   Equals(_evolveDuration,
                       durations._evolveDuration);
        }

        public override int GetHashCode()
        {
            int hash;
            hash = 17;
            hash += 31 * _offspringSelectionDuration.GetHashCode() + 17;
            hash += 31 * _survivorsSelectionDuration.GetHashCode() + 17;
            hash += 31 * _offspringAlterDuration.GetHashCode() + 17;
            hash += 31 * _offspringFilterDuration.GetHashCode() + 17;
            hash += 31 * _survivorFilterDuration.GetHashCode() + 17;
            hash += 31 * _evaluationDuration.GetHashCode() + 17;
            hash += 31 * _evolveDuration.GetHashCode() + 17;
            return hash;
        }

        public static EvolutionDurations Of(
            TimeSpan offspringSelectionDuration,
            TimeSpan survivorsSelectionDuration,
            TimeSpan offspringAlterDuration,
            TimeSpan offspringFilterDuration,
            TimeSpan survivorFilterDuration,
            TimeSpan evaluationDuration,
            TimeSpan evolveDuration
        )
        {
            return new EvolutionDurations(
                offspringSelectionDuration,
                survivorsSelectionDuration,
                offspringAlterDuration,
                offspringFilterDuration,
                survivorFilterDuration,
                evaluationDuration,
                evolveDuration
            );
        }
    }
}