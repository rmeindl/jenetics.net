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

using System.Runtime.CompilerServices;
using Jenetics.Util;

namespace Jenetics.Internal.Collection
{
    public class ArraySeqTest : SeqTestBase
    {
        protected override ISeq<int> NewSeq(int length)
        {
            var impl = Array.OfLength<int>(length);
            for (var i = 0; i < length; ++i)
                impl[i] = i;
            return new ArraySeqImpl<int>(impl);
        }

        private class ArraySeqImpl<T> : ArraySeqBase<T>
        {
            public ArraySeqImpl(Array<T> array) : base(array.Seal())
            {
            }
        }

    }
}