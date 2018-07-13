# Requirements
* Requires NuGet 3.3 or higher for .NET Classic plugin and NuGet 3.5 or higher for .NET Core plugin [Download](https://www.nuget.org/downloads)
* Requires .NET Core SDK 2.1.300 or later versions installed (**Only for .NET Core plugin**) [Download](https://www.microsoft.com/net/download)
* Projects **must** have a policy file named **.osda**

<details><summary>Policy file structure</summary>
<p>

```
{
  "$schema": "http://json-schema.org/draft-04/schema#",
  "title": "Project Policy",
  "description": "A policy with a project related configurations and \tinformation",
  "type": "object",
  "properties": {
    "project_id": {
      "description": "Id of the project to present in the report",
      "type": "string"
    },
    "project_name": {
      "description": "Name of the project to present in the report",
      "type": "string"
    },
    "project_version": {
      "description": "Version of the project to present in the report",
      "type": "string"
    },
    "project_description": {
      "description": "Description of the project to present in the report",
      "type": "string"
    },
    "organization": {
      "description": "The organization the project belongs to",
      "type": "string"
    },
    "repo": {
      "description": "The repository in github the project belongs to",
      "type": "string"
    },
    "repo_owner": {
      "description": "The owner of the repository the project belongs to",
      "type": "string"
    },
    "admin": {
      "description": "The username of the administrator of the project (Only used in project first report)",
      "type": "string"
    },
    "invalid_licenses": {
      "description": "The names of all invalid licenses. Default value is an empty collection",
      "type": "array"
    },
    "api_cache_time": {
      "description": "Indicates, in seconds, the amount of time the cached results should be considered valid. If 0 (which is the default value), there are no restrictions on the lifetime of cached results",
      "type": "number"
    }
  },
  "required": ["project_id", "project_name", "admin"]
}
```

</p>
</details>

# Plugin for .NET Classic projects

[![NuGet](https://img.shields.io/nuget/v/DotnetDependencyAnalyzer.svg)](https://www.nuget.org/packages/DotnetDependencyAnalyzer/)
[![license](https://img.shields.io/github/license/pt-osda/dotnet-dependency-analyzer.svg)](https://github.com/pt-osda/dotnet-dependency-analyzer/blob/master/LICENSE)

Analyze Open Source dependencies used in .NET Classic projects.

## How to install

This package is available via NuGet and can be installed using the following command:
```
Install-Package DotnetDependencyAnalyzer
```

## How to execute plugin

After installed, a batch file named DependencyAnalyzer is placed in the solution folder. This file analyzes all projects of the solution. To execute the plugin, execute the batch file:
```
DependencyAnalyzer.bat
```

:warning: In order for the plugin to be executed successfully, projects must be built before because plugin uses information contained on solution packages folder.




# Plugin for .NET Core projects

[![NuGet](https://img.shields.io/nuget/v/dotnet-dependency-analyzer.svg)](https://www.nuget.org/packages/dotnet-dependency-analyzer/)
[![license](https://img.shields.io/github/license/pt-osda/dotnet-dependency-analyzer.svg)](https://github.com/pt-osda/dotnet-dependency-analyzer/blob/master/LICENSE)

Analyze Open Source dependencies used in .NET Core projects.

## How to install

This package is available via NuGet and can be installed using one of the following commands:
```
 dotnet tool install -g dotnet-dependency-analyzer // 1)
 dotnet tool install dotnet-dependency-analyzer --tool-path <path> // 2)
```
**1)** Plugin is installed globally on the machine

**2)** Plugin is installed on a specific folder (tool-path)

Other helpful commands (e.g update or uninstall package) can be found [here](https://docs.microsoft.com/pt-pt/dotnet/core/tools/global-tools#other-cli-commands).

## How to execute plugin

```
dotnet-dependency-analyzer <project-path>

```
* **project-path**: optional parameter. If not specified, the plugin will search for a project in the current directory of the command line. Otherwise, the plugin will search for a project in the specified path.

:warning: In order for the plugin to be executed successfully, projects must be built before because plugin uses information contained on solution packages folder.
