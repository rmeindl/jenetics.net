# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade core\Jenetics\Jenetics.csproj
4. Upgrade core\Jenetics.Tests\Jenetics.Tests.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|
| None                                           | Not applicable              |

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### core\Jenetics\Jenetics.csproj modifications

Project properties changes:
  - Target framework should be changed from `netcoreapp2.0` to `net10.0`

#### core\Jenetics.Tests\Jenetics.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `netcoreapp2.0` to `net10.0`
