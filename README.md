# Jenetics.Net

**Jenetics.Net** is a port of the popular Java Genetic Algorithm framework [Jenetics](http://jenetics.io) to .NET.

## Documentation

Currently the documentation is not ported, but because of the nearly identical structure, look into the Jenetics
([javadoc](http://jenetics.io/javadoc/jenetics/3.9/index.html)) and the user manual ([pdf](http://jenetics.io/manual/manual-3.9.0.pdf)).

## Requirements

### Build time

* **.NET 10**: The [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) must be installed.

## Build Jenetics.Net

Clone the repository and change into its directory:

```sh
git clone https://github.com/rmeindl/jenetics.net.git
cd jenetics.net
```

The solution builds the library and test project:

```sh
dotnet build src/Jenetics.sln
```

To build the library alone or run its xUnit tests:

```sh
dotnet build src/core/Jenetics/Jenetics.csproj
dotnet test src/core/Jenetics.Tests/Jenetics.Tests.csproj
```

Build and test outputs are written under each project's `bin\Debug\net10.0` directory.

The example projects are standalone and are not included in the solution. Run an example from the repository root,
for example:

```sh
dotnet run --project src/examples/HelloWorld/HelloWorld.csproj
```

To publish an example as a self-contained application, choose a [runtime identifier](https://learn.microsoft.com/dotnet/core/rid-catalog):

```sh
dotnet publish src/examples/HelloWorld/HelloWorld.csproj -r linux-x64 --self-contained
dotnet publish src/examples/HelloWorld/HelloWorld.csproj -r win-x64 --self-contained
```

With the default Release configuration, publish output is under
`src/examples/HelloWorld/bin/Release/net10.0/<RID>/publish`.

## Example

### Hello World (Ones counting)

The minimum engine setup needs a genotype factory (`Factory<Genotype<TGene>>`) and a fitness function. `Genotype<TGene>`
implements `IFactory<Genotype<TGene>>` and can be used as a prototype for creating new genotypes.

```cs
using System;
using System.Linq;
using Jenetics.Engine;
using JeneticsEngine = Jenetics.Engine.Engine;

namespace Jenetics.Example;

public static class HelloWorld
{
    // 2.) Definition of the fitness function.
    private static int Eval(Genotype<BitGene> gt)
    {
        return gt.GetChromosome().As<BitChromosome>().BitCount();
    }

    public static void Main()
    {
        // 1.) Define the genotype factory suitable for the problem.
        Genotype<BitGene> F()
        {
            return Genotype.Of(BitChromosome.Of(10, 0.5));
        }

        // 3.) Create the execution environment.
        var engine = JeneticsEngine.Builder(Eval, F).Build();

        // 4.) Start the execution (evolution) and collect the result.
        var result = engine.Stream().Take(100).ToBestGenotype();

        Console.WriteLine($"Hello World:\n{result}");
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