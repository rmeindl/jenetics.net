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
using System.Linq;
using Jenetics.Util;
using Xunit;

namespace Jenetics.Internal.Math
{
    public class DoubleAdderTest : ObjectTesterBase<DoubleAdder>
    {
        protected override Factory<DoubleAdder> Factory()
        {
            return () =>
            {
                var random = RandomRegistry.GetRandom();
                var adder = new DoubleAdder();
                for (var i = 0; i < 20; ++i)
                    adder.Add(random.NextDouble());

                return adder;
            };
        }

        [Fact]
        public void Add()
        {
            const int size = 1_000_000;
            var random = new Random(12349);
            var numbers = new List<double>(size);

            for (var i = 1; i <= size; ++i)
                numbers.Add(random.NextDouble() * i * 1_000_000);

            var adder = new DoubleAdder();
            foreach (var n in numbers)
                adder.Add(n);

            var expectedSum = numbers.Sum();

            Assert.True(expectedSum > adder.DoubleValue);
        }

        [Fact]
        public void SameState()
        {
            var da1 = RandomRegistry.With(new Random(589), r => Factory()());
            var da2 = RandomRegistry.With(new Random(589), r => Factory()());

            Assert.True(da1.SameState(da2));

            var random = new Random();
            for (var i = 0; i < 100; ++i)
            {
                var value = random.NextDouble();
                da1.Add(value);
                da2.Add(value);

                Assert.True(da1.SameState(da2));
            }
        }
    }
}