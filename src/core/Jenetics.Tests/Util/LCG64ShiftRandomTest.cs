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

using System;
using Jenetics.Util;
using Xunit;

#pragma warning disable CS0618

namespace Jenetics;

public class LCG64ShiftRandomTest
{
    [Fact]
    public void FirstOutputMatchesReferenceValue()
    {
        var random = new LCG64ShiftRandom(42);

        Assert.Equal(7347731091452081471L, random.NextLong());
    }

    [Fact]
    public void SameSeedProducesSameSequence()
    {
        var first = new LCG64ShiftRandom(42);
        var second = new LCG64ShiftRandom(42);

        for (var i = 0; i < 100; ++i)
            Assert.Equal(first.NextLong(), second.NextLong());
    }

    [Fact]
    public void JumpMatchesRepeatedSteps()
    {
        var jumped = new LCG64ShiftRandom(42);
        var stepped = new LCG64ShiftRandom(42);

        jumped.Jump(100);
        for (var i = 0; i < 100; ++i)
            stepped.NextLong();

        Assert.Equal(stepped.NextLong(), jumped.NextLong());
    }

    [Fact]
    public void Jump2MatchesPowerOfTwoSteps()
    {
        var jumped = new LCG64ShiftRandom(42);
        var stepped = new LCG64ShiftRandom(42);

        jumped.Jump2(4);
        stepped.Jump(16);

        Assert.Equal(stepped.NextLong(), jumped.NextLong());
    }

    [Fact]
    public void SplitRejectsNegativeStreamIndex()
    {
        var random = new LCG64ShiftRandom(42);

        Assert.Throws<ArgumentOutOfRangeException>(() => random.Split(2, -1));
    }
}

#pragma warning restore CS0618
