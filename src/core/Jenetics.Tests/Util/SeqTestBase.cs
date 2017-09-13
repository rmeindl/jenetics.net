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
using System.Reflection;
using Xunit;

namespace Jenetics.Util
{
    public abstract class SeqTestBase
    {
        protected abstract ISeq<int> NewSeq(int length);

        private static Func<int, bool> ValueOf(int value)
        {
            return i => i == value;
        }

        [Fact]
        public void Length()
        {
            for (var i = 0; i < 100; ++i)
            {
                var seq = NewSeq(i);
                for (var j = 0; j < i; ++j)
                    Assert.Equal(j, seq[j]);
                Assert.Equal(i, seq.Length);
            }
        }
    }
}