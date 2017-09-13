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
using System.Threading;

namespace Jenetics.Util
{
    public class Context<T>
    {
        private readonly T _default;
        private readonly ThreadLocal<Entry> _threadLocalEntry = new ThreadLocal<Entry>();
        private Entry _entry;

        internal Context(T defaultValue)
        {
            _default = defaultValue;
            _entry = new Entry(defaultValue);
        }

        internal void Set(T value)
        {
            var e = _threadLocalEntry.Value;

            if (e != null) e.Value = value;
            else Volatile.Write(ref _entry, new Entry(value));
        }

        internal T Get()
        {
            var e = _threadLocalEntry.Value;
            return (e != null ? e : Volatile.Read(ref _entry)).Value;
        }

        internal void Reset()
        {
            Set(_default);
        }

        internal TR With<TR>(T value, Func<T, TR> f)
        {
            var e = _threadLocalEntry.Value;
            if (e != null)
                _threadLocalEntry.Value = e.Inner(value);
            else
                _threadLocalEntry.Value = new Entry(value, Thread.CurrentThread);

            try
            {
                return f(value);
            }
            finally
            {
                _threadLocalEntry.Value = _threadLocalEntry.Value.Parent;
            }
        }

        private class Entry
        {
            private readonly Thread _thread;
            internal readonly Entry Parent;
            internal T Value;

            private Entry(T value, Entry parent, Thread thread)
            {
                Value = value;
                Parent = parent;
                _thread = thread;
            }

            public Entry(T value, Thread thread) : this(value, null, thread)
            {
            }

            public Entry(T value) : this(value, null, null)
            {
            }

            public Entry Inner(T value)
            {
                return new Entry(value, this, _thread);
            }
        }
    }
}