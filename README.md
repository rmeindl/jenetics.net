# Jenetics.Net

**Jenetics.Net** is a port of the popular Java Genetic Algorithm framework [Jenetics](http://jenetics.io) to .NET Core.

## Documentation

Currently the documentation is not ported, but because of the nearly identical structure, look into the Jenetics ([javadoc](http://jenetics.io/javadoc/jenetics/3.9/index.html)) and the user manual ([pdf](http://jenetics.io/manual/manual-3.9.0.pdf)).

## Requirements

### Build time
*  **.Net Core 2.0**: The [.Net Core 2.0](https://www.microsoft.com/net/download/core) SDK must be installed.

## Build Jenetics.Net

Check out the master branch from Github.

    $ git clone https://github.com/rmeindl/jenetics.net.git <builddir>

Jenetics.Net is currently not using anything thing else for building besides .Net Core projects.

Go to `src/core/Jenetics` for building the library

    $ dotnet build
    
Go to `src/core/Jenetics.Tests` for running the xUnit Tests

    $ dotnet xunit
    
All artifacts can be found in `bin\Debug\netcoreapp2.0`.

Go to `src/examples/<example>` for running on of the examples, e.g. `src/examples/HelloWorld`

    $ dotnet run
        
If you want to publish an example including the .Net Core runtime as self-contained application, define the [runtime](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog)

    $ dotnet publish -r linux-x64 --self-contained

or 

    $ dotnet publish -r win-x64 --self-contained

All artifacts can be found in `bin\Debug\netcoreapp2.0\linux-x64\publish` or `bin\Debug\netcoreapp2.0\win-x64\publish`.

## Example

### Hello World (Ones counting)

The minimum evolution Engine setup needs a genotype factory, `Factory<Genotype<TGene>>`, and a fitness `Function`. The `Genotype` implements the `Factory` interface and can therefore be used as prototype for creating the initial `Population` and for creating new random `Genotypes`.

```cs
using System;
using System.Linq;
using Jenetics.Engine;

namespace Jenetics.Example
{
    public static class HelloWorld
    {
        // 2.) Definition of the fitness function.
        private static int Eval(Genotype<BitGene> gt)
        {
            return gt.GetChromosome().As<BitChromosome>().BitCount();
        }

        public static void Main()
        {
            // 1.) Define the genotype (factory) suitable
            //     for the problem.
            Genotype<BitGene> F()
            {
                return Genotype.Of(BitChromosome.Of(10, 0.5));
            }

            // 3.) Create the execution environment.
            var engine = Engine.Engine.Builder(Eval, F).Build();

            // 4.) Start the execution (evolution) and
            //     collect the result.
            var result = engine.Stream().Take(100).ToBestGenotype();

            Console.WriteLine("Hello World:\n" + result);
        }
    }
}
```

## License

The library is licensed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0.html).

	Copyright 2007-2017 Franz Wilhelmstötter

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
	
## Release notes
	
_[All Release Notes](RELEASE_NOTES.md)_