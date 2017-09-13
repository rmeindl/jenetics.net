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

namespace Jenetics.Stat
{
    public class IntMomentStatistics : MomentStatistics
    {
        public int Min { get; private set; } = int.MaxValue;

        public int Max { get; private set; } = int.MinValue;

        public long Sum { get; private set; }

        public void Accept(int value)
        {
            base.Accept(value);
            Min = Math.Min(Min, value);
            Max = Math.Max(Max, value);
            Sum += value;
        }
    }
}