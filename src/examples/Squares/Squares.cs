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
using System.Linq;
using Jenetics.Engine;
using Jenetics.Util;

namespace Jenetics.Example
{
    public static class Squares
    {
        private static Dimension NextDimension()
        {
            var random = RandomRegistry.GetRandom();
            return new Dimension(random.NextInt(100), random.NextInt(100));
        }

        private static double Area(Dimension dim)
        {
            return dim.Height * dim.Width;
        }

        public static void Main()
        {
            var engine = Engine.Engine
                .Builder(Area, Codecs.OfScalar(NextDimension))
                .Build();

            var pt = engine.Stream()
                .Take(50)
                .ToBestPhenotype();

            Console.WriteLine(pt);
        }

        private class Dimension
        {
            public Dimension(int width, int height)
            {
                Width = width;
                Height = height;
            }

            public int Width { get; }
            public int Height { get; }
        }
    }
}