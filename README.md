# HansPeterGit

[![NuGet](https://img.shields.io/nuget/v/HansPeterGit.svg)](https://nuget.org/packages/HansPeterGit/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build Status](https://github.com/twenzel/HansPeterGit/workflows/CI/badge.svg?branch=main)](https://github.com/twenzel/HansPeterGit/actions)

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=twenzel_HansPeterGit&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=twenzel_HansPeterGit)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=twenzel_HansPeterGit&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=twenzel_HansPeterGit)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=twenzel_HansPeterGit&metric=security_rating)](https://sonarcloud.io/dashboard?id=twenzel_HansPeterGit)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=twenzel_HansPeterGit&metric=bugs)](https://sonarcloud.io/dashboard?id=twenzel_HansPeterGit)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=twenzel_HansPeterGit&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=twenzel_HansPeterGit)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=twenzel_HansPeterGit&metric=coverage)](https://sonarcloud.io/dashboard?id=twenzel_HansPeterGit)

GIT interface for .NET

This repository contains a cross-platform interface to the git CLI (aka git.exe/git).

The project aim is to get rid of the stale library `libgit2sharp` and provide an easy interface to interact with the git cli.

## Dependencies

An installed version of git should be accessible via path or can be defined via the `GitOptions.PathToGit` property.

## Install

Add the NuGet package [HansPeterGit](https://nuget.org/packages/HansPeterGit/) to any project supporting .NET Standard 2.0 or higher.

> &gt; dotnet add package HansPeterGit

## Usage

### Clone a repository

**GitRepository.Clone()** can download a remote repository to the local file system, same as the git clone command.

```csharp
GitRepository.Clone("https://github.com/twenzel/HansPeterGit.git", @"D:\TestSample");
```

### Create a local repository

The **GitRepository.Init()** method can create a new Git repository in the specified path, equivalent to the git init command.

```csharp
var repos = new GitRepository(@"D:\TestSample");
repos.Init();
```

### Get status

Using the **GetStatus()** method to retrieve the repositories current status.

```csharp
var repos = new GitRepository(@"D:\TestSample");
var status = repos.GetStatus();

Console.WriteLine($"Current branch: {status.Branch}");
Console.WriteLine($"Current commit: {status.Commit}");

if (status.IsDirty)
{
    var newfile = status.Added.First();
}

var fileInfo = status["src/newFile.json"];

```

### Staging/Restore

To stage the current work tree use any of the **Stage/Add/Restore** methods.

```csharp
repos.StageAll();

// Stage file
repos.Add("src/newFile.json");

// Discard changes in working directory
repos.Restore("src/newFile.json");

// Unstage file
repos.Unstage("src/newFile.json");
```

### Commit changes

The **Commit()** method can be used to commit the current changes.

```csharp
var commit = repos.Commit("Fix calculation bug");

// with dedicated author
repos.Commit("Fix calculation bug", new Author("Some name", "bugfixer@test.com"));
```

### Push changes

To push changes from the local repository please use the **Push()** methods.

```csharp
repos.Push();

// with dedicated remove name
repos.Push("origin", "main");

// with setting an upstream
repos.PushWithUpstream("origin", "main");
```

### Others

There are already some other methods but not all commands are wrapped/implemented yet. Please feel free to create an issue or contribute.

### Authentication

Sometimes the remote repository requires an authentication (e.g. GitHub, Azure DevOps). In order to clone or push the repository you can define an authentication.

```csharp
var options = new GitOptions(workingDirectory);
options.Authentication = new BasicAuthentication("pat", "myuserpat"); 

var repository = GitRepository.Clone("https://dev.azure.com/yourOrgName/yourProject/_git/yourRepository", options);
```

Beside `BasicAuthentication` an `BearerAuthentication` is also already build-in. You can implement your own authentication by implementing the `IAuthentication` interface.
The build-in authentication implementation uses the '-c http.extraheader' argument of the git cli to provide the credentials.
