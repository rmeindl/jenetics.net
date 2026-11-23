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
using Jenetics.Engine;
using Xunit;

namespace Jenetics.Engine;

public class TimingParityTest
{
    [Fact]
    public void TimerUsesConfiguredTimeProvider()
    {
        var timeProvider = new ManualTimeProvider();
        var timer = Timer.Of(timeProvider).Start();

        timeProvider.Advance(TimeSpan.FromSeconds(2));
        timer.Stop();

        Assert.Equal(TimeSpan.FromSeconds(2), timer.GetTime());
    }

    [Fact]
    public void EngineAndClonedBuilderRetainTimeProvider()
    {
        var timeProvider = new ManualTimeProvider();
        var builder = Engine.Builder<BitGene, int>(
            _ => 1,
            () => Genotype.Of(BitChromosome.Of(2))
        ).TimeProvider(timeProvider);

        var engine = builder.Build();

        Assert.Same(timeProvider, builder.GetTimeProvider());
        Assert.Same(timeProvider, engine.GetTimeProvider());
        Assert.Same(timeProvider, engine.Builder().GetTimeProvider());
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        private long _timestamp;

        public override long TimestampFrequency => TimeSpan.TicksPerSecond;

        public override long GetTimestamp()
        {
            return _timestamp;
        }

        public void Advance(TimeSpan duration)
        {
            _timestamp += duration.Ticks;
        }
    }
}
