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

namespace Jenetics.Engine;

/// <summary>
///     Represents the starting state for an evolutionary algorithm: an initial population and generation number.
///     Modernized with primary constructor and init-only properties (C# 12).
/// </summary>
public class EvolutionStart<TGene, TAllele>(
    Population<TGene, TAllele> population,
    long generation
) where TGene : IGene<TGene> where TAllele : IComparable<TAllele>
{
    public Population<TGene, TAllele> Population { get; } = population;
    public long Generation { get; } = generation;
}

public static class EvolutionStart
{
    public static EvolutionStart<TGene, TAllele> Of<TGene, TAllele>(Population<TGene, TAllele> population,
        long generation)
        where TGene : IGene<TGene>
        where TAllele : IComparable<TAllele>
    {
        return new EvolutionStart<TGene, TAllele>(population, generation);
    }
}