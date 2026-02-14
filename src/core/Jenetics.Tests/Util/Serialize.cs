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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;
using Xunit.Sdk;

namespace Jenetics.Util
{
    public static class Serialize
    {
        public static void Test<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
#pragma warning disable SYSLIB0011 // BinaryFormatter is used to validate legacy serialization behavior.
                IFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(stream, obj);
                }
                catch (PlatformNotSupportedException)
                {
                    // BinaryFormatter is not supported on this runtime (e.g., .NET 10+)
                    // Re-throw to allow calling code to handle appropriately
                    throw;
                }

                stream.Seek(0, SeekOrigin.Begin);

                var copy = formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011 // BinaryFormatter is used to validate legacy serialization behavior.

                Assert.Equal(obj, copy);
            }
        }
    }
}