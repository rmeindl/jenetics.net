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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Jenetics.Util
{
    public class TestData : IEnumerable<string[]>
    {
        private readonly string[] _parameters;
        private readonly string _resource;

        private TestData(string resource, params string[] parameters)
        {
            _resource = resource;
            _parameters = parameters;
        }

        public IEnumerator<string[]> GetEnumerator()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(GetResourcePath()))
            {
                using (var reader = new StreamReader(stream))
                {
                    while (true)
                    {
                        var line = reader.ReadLine();
                        if (line == null)
                            yield break;
                        if (!line.Trim().StartsWith("#") && line.Trim().Length != 0)
                            yield return line.Split(",");
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string GetResourcePath()
        {
            var param = _parameters.Length == 0 ? "" : "[" + string.Join(",", _parameters) + "]";
            return _resource + param + ".dat";
        }

        public static TestData Of(string resource, params string[] parameters)
        {
            return new TestData(resource, parameters);
        }

        public static double[] ToDouble(string[] line)
        {
            return line.Select(l => double.Parse(l)).ToArray();
        }
    }
}