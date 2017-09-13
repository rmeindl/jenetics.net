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
    public static class TimedResult
    {
        public static Func<TimedResult<T>> Of<T>(Func<T> supplier)
        {
            return () =>
            {
                var timer = Timer.Of().Start();
                var result = supplier();
                return new TimedResult<T>(timer.Stop().GetTime(), result);
            };
        }
    }

    public class TimedResult<T>
    {
        internal readonly T Result;
        internal TimeSpan Duration;

        public TimedResult(TimeSpan duration, T result)
        {
            Duration = duration;
            Result = result;
        }
    }
}