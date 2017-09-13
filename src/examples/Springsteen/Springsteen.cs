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
using static Jenetics.Internal.Util.Require;

namespace Jenetics.Example
{
    public class Springsteen : IProblem<IImmutableSeq<Springsteen.Record>, BitGene, double>
    {
        private readonly double _maxPricePerUniqueSong;

        private readonly IImmutableSeq<Record> _records;

        private Springsteen(IImmutableSeq<Record> records, double maxPricePerUniqueSong)
        {
            _records = NonNull(records);
            _maxPricePerUniqueSong = maxPricePerUniqueSong;
        }

        public Func<IImmutableSeq<Record>, double> Fitness()
        {
            return records =>
            {
                var cost = records
                    .Select(r => r.Price)
                    .Sum();

                var uniqueSongCount = records
                    .Select(r => r.Songs)
                    .ToHashSet()
                    .Count;

                var pricePerUniqueSong = cost / uniqueSongCount;

                return pricePerUniqueSong <= _maxPricePerUniqueSong
                    ? uniqueSongCount
                    : 0.0;
            };
        }

        public ICodec<IImmutableSeq<Record>, BitGene> Codec()
        {
            return Codecs.OfSubSet(_records);
        }

        public static void Main()
        {
            const double maxPricePerUniqueSong = 2.5;

            var springsteen = new Springsteen(
                ImmutableSeq.Of(
                    new Record("Record1", 25, ImmutableSeq.Of("Song1", "Song2", "Song3", "Song4", "Song5", "Song6")),
                    new Record("Record2", 15, ImmutableSeq.Of("Song2", "Song3", "Song4", "Song5", "Song6", "Song7")),
                    new Record("Record3", 35, ImmutableSeq.Of("Song5", "Song6", "Song7", "Song8", "Song9", "Song10")),
                    new Record("Record4", 17,
                        ImmutableSeq.Of("Song9", "Song10", "Song12", "Song4", "Song13", "Song14")),
                    new Record("Record5", 29,
                        ImmutableSeq.Of("Song1", "Song2", "Song13", "Song14", "Song15", "Song16")),
                    new Record("Record6", 5, ImmutableSeq.Of("Song18", "Song20", "Song30", "Song40"))
                ),
                maxPricePerUniqueSong
            );

            var engine = Engine.Engine.Builder(springsteen).Build();

            var result = springsteen.Codec().Decoder()(
                engine.Stream()
                    .Take(10)
                    .ToBestGenotype()
            );

            var cost = result
                .Select(r => r.Price)
                .Sum();

            var uniqueSongCount = result
                .Select(r => r.Songs)
                .ToHashSet()
                .Count;

            var pricePerUniqueSong = cost / uniqueSongCount;

            Console.WriteLine("Overall cost:  " + cost);
            Console.WriteLine("Unique songs:  " + uniqueSongCount);
            Console.WriteLine("Cost per song: " + cost / uniqueSongCount);
            Console.WriteLine("Records:       " + string.Join(", ", result));
        }

        public class Record
        {
            public double Price { get; }
            public IImmutableSeq<string> Songs { get; }
            public string Name { get; }

            public Record(string name, double price, IImmutableSeq<string> songs)
            {
                Name = NonNull(name);
                Price = price;
                Songs = NonNull(songs);
            }
        }
    }
}