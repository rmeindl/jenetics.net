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
using Jenetics.Util;

namespace Jenetics.Internal.Util
{
    public interface IHash
    {
        int Value { get; }
        IHash And(byte[] values);
        IHash And(char[] values);
        IHash And<T>(List<T> values);
        IHash And<T>(ISeq<T> values);
        IHash And<T>(IImmutableSeq<T> values);
        IHash And(char value);
        IHash And(double value);
        IHash And(int value);
        IHash And(long value);
        IHash And<T>(T obj);
    }

    public static class Hash
    {
        public static IHash Of(Type type)
        {
            return new DefaultHashCodeBuilder(type);
        }
    }
}