# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [core\Jenetics.Tests\Jenetics.Tests.csproj](#corejeneticstestsjeneticstestscsproj)
  - [core\Jenetics\Jenetics.csproj](#corejeneticsjeneticscsproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 2 | All require upgrade |
| Total NuGet Packages | 5 | All compatible |
| Total Code Files | 180 |  |
| Total Code Files with Incidents | 2 |  |
| Total Lines of Code | 17767 |  |
| Total Number of Issues | 2 |  |
| Estimated LOC to modify | 0+ | at least 0,0% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :--- |
| [core\Jenetics.Tests\Jenetics.Tests.csproj](#corejeneticstestsjeneticstestscsproj) | netcoreapp2.0 | 🟢 Low | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [core\Jenetics\Jenetics.csproj](#corejeneticsjeneticscsproj) | netcoreapp2.0 | 🟢 Low | 0 | 0 |  | ClassLibrary, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 5 | 100,0% |
| ⚠️ Incompatible | 0 | 0,0% |
| 🔄 Upgrade Recommended | 0 | 0,0% |
| ***Total NuGet Packages*** | ***5*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 446 |  |
| ***Total APIs Analyzed*** | ***446*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| MathNet.Numerics.Core | 3.17.0 |  | [Jenetics.Tests.csproj](#corejeneticstestsjeneticstestscsproj) | ✅Compatible |
| Microsoft.NET.Test.Sdk | 15.3.0 |  | [Jenetics.Tests.csproj](#corejeneticstestsjeneticstestscsproj) | ✅Compatible |
| Microsoft.NETCore.App |  |  | [Jenetics.csproj](#corejeneticsjeneticscsproj)<br/>[Jenetics.Tests.csproj](#corejeneticstestsjeneticstestscsproj) | ✅Compatible |
| xunit | 2.3.0-beta5-build3769 |  | [Jenetics.Tests.csproj](#corejeneticstestsjeneticstestscsproj) | ✅Compatible |
| xunit.runner.visualstudio | 2.3.0-beta5-build3769 |  | [Jenetics.Tests.csproj](#corejeneticstestsjeneticstestscsproj) | ✅Compatible |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;Jenetics.Tests.csproj</b><br/><small>netcoreapp2.0</small>"]
    P2["<b>📦&nbsp;Jenetics.csproj</b><br/><small>netcoreapp2.0</small>"]
    P1 --> P2
    click P1 "#corejeneticstestsjeneticstestscsproj"
    click P2 "#corejeneticsjeneticscsproj"

```

## Project Details

<a id="corejeneticstestsjeneticstestscsproj"></a>
### core\Jenetics.Tests\Jenetics.Tests.csproj

#### Project Info

- **Current Target Framework:** netcoreapp2.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 1
- **Dependants**: 0
- **Number of Files**: 84
- **Number of Files with Incidents**: 1
- **Lines of Code**: 7249
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["Jenetics.Tests.csproj"]
        MAIN["<b>📦&nbsp;Jenetics.Tests.csproj</b><br/><small>netcoreapp2.0</small>"]
        click MAIN "#corejeneticstestsjeneticstestscsproj"
    end
    subgraph downstream["Dependencies (1"]
        P2["<b>📦&nbsp;Jenetics.csproj</b><br/><small>netcoreapp2.0</small>"]
        click P2 "#corejeneticsjeneticscsproj"
    end
    MAIN --> P2

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 446 |  |
| ***Total APIs Analyzed*** | ***446*** |  |

<a id="corejeneticsjeneticscsproj"></a>
### core\Jenetics\Jenetics.csproj

#### Project Info

- **Current Target Framework:** netcoreapp2.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 1
- **Number of Files**: 110
- **Number of Files with Incidents**: 1
- **Lines of Code**: 10518
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P1["<b>📦&nbsp;Jenetics.Tests.csproj</b><br/><small>netcoreapp2.0</small>"]
        click P1 "#corejeneticstestsjeneticstestscsproj"
    end
    subgraph current["Jenetics.csproj"]
        MAIN["<b>📦&nbsp;Jenetics.csproj</b><br/><small>netcoreapp2.0</small>"]
        click MAIN "#corejeneticsjeneticscsproj"
    end
    P1 --> MAIN

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

