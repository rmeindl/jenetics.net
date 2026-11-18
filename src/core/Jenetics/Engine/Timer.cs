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

namespace Jenetics.Engine;

public class Timer
{
    private readonly Func<long> _nanoClock;
    private readonly Func<long, long, TimeSpan> _elapsedTime;

    private long _start;
    private long _stop;

    public Timer(Func<long> nanoClock)
        : this(nanoClock, (start, stop) => TimeSpan.FromTicks(stop - start))
    {
    }

    private Timer(
        Func<long> nanoClock,
        Func<long, long, TimeSpan> elapsedTime
    )
    {
        _nanoClock = nanoClock ?? throw new ArgumentNullException(nameof(nanoClock));
        _elapsedTime = elapsedTime ?? throw new ArgumentNullException(nameof(elapsedTime));
    }

    public Timer Start()
    {
        _start = _nanoClock();
        return this;
    }

    public Timer Stop()
    {
        _stop = _nanoClock();
        return this;
    }

    public TimeSpan GetTime()
    {
        return _elapsedTime(_start, _stop);
    }

    public static Timer Of()
    {
        return Of(TimeProvider.System);
    }

    public static Timer Of(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        return new Timer(
            timeProvider.GetTimestamp,
            (start, stop) => timeProvider.GetElapsedTime(start, stop)
        );
    }
}