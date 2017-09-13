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
using Jenetics.Test;
using Xunit;
using static Jenetics.Util.RandomRegistry;

namespace Jenetics.Util
{
    public abstract class ObjectTesterBase<T> : RetryBase
        where T : class
    {
        protected abstract Factory<T> Factory();


        protected IMutableSeq<T> NewEqualObjects(int length)
        {
            IEnumerable<T> F()
            {
                for (var i = 0; i < length; i++)
                    yield return With(new Random(589), r => Factory()());
            }

            return F().ToMutableSeq();
        }

        [Fact]
        public void CloneMethod()
        {
            var that = Factory()();

            if (that is ICloneable)
            {
                var clone = that.GetType().GetMethod("clone");
                var other = clone.Invoke(that, null);

                Assert.Equal(that, other);
                Assert.NotSame(that, other);
            }
        }

        [Fact]
        public void CopyMethod()
        {
            var that = Factory()();
            if (that is ICopyable<T> copyable)
            {
                var other = copyable.Copy();
                if (other.GetType() == copyable.GetType())
                {
                    Assert.Equal(that, other);
                    Assert.NotSame(copyable, other);
                }
            }
        }

        [Fact]
        public void Equal()
        {
            var same = NewEqualObjects(5);

            var that = same[0];
            for (var i = 1; i < same.Length; ++i)
            {
                var other = same[i];

                Assert.Equal(other, other);
                Assert.Equal(that, other);
                Assert.Equal(other, that);
                Assert.Equal(other.GetHashCode(), that.GetHashCode());
            }
        }

        [Fact]
        public void HashCodeMethod()
        {
            var same = NewEqualObjects(5);

            var that = same[0];
            for (var i = 1; i < same.Length; ++i)
            {
                var other = same[i];

                Assert.Equal(other.GetHashCode(), that.GetHashCode());
            }
        }

        [Fact]
        public virtual void IsValid()
        {
            var a = Factory()();
            if (a is IVerifiable verifiable)
                Assert.True(verifiable.IsValid);
        }

        [Fact]
        public void NotEquals()
        {
            for (var i = 0; i < 10; ++i)
            {
                var that = Factory()();
                var other = Factory()();

                if (that.Equals(other))
                {
                    Assert.True(other.Equals(that));
                    Assert.Equal(other.GetHashCode(), that.GetHashCode());
                }
                else
                {
                    Assert.False(other.Equals(that));
                    Assert.False(that.Equals(other));
                }
            }
        }

        [Fact]
        public void NotEqualsClassType()
        {
            var that = Factory()();
            Assert.False(that.Equals(typeof(Type)));
        }

        [Fact]
        public void NotEqualsNull()
        {
            var that = Factory()();
            Assert.False(that == null);
        }

        [Fact]
        public void NotEqualsStringType()
        {
            var that = Factory()();
            Assert.False(that.Equals("__some_string__"));
        }

        [Fact]
        public virtual void ObjectSerialize()
        {
            var type = Factory()().GetType();

            if (!type.IsSerializable) return;

            for (var i = 0; i < 10; ++i)
            {
                var serializable = Factory()();

                Serialize.Test(serializable);
            }
        }

        [Fact]
        public void ToStringMethod()
        {
            var same = NewEqualObjects(5);

            var that = same[0];
            for (var i = 1; i < same.Length; ++i)
            {
                var other = same[i];

                Assert.Equal(that.ToString(), other.ToString());
                Assert.NotNull(other.ToString());
            }
        }
    }
}