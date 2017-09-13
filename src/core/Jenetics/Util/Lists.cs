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
using System.Collections.Generic;

namespace Jenetics.Util
{
    public static class Lists
    {
        public static void Shuffle<T>(IList<T> list)
        {
            Shuffle(list, RandomRegistry.GetRandom());
        }

        public static void Shuffle<T>(IList<T> list, Random random)
        {
            for (var j = list.Count - 1; j > 0; --j)
                Swap(list, j, random.NextInt(j + 1));
        }

        public static void Swap<T>(IList<T> list, int i, int j)
        {
            var old = list[i];
            list[i] = list[j];
            list[j] = old;
        }
    }
}