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

An installed version of git should be accessible via path.

## Install

Add the NuGet package [HansPeterGit](https://nuget.org/packages/HansPeterGit/) to any project supporting .NET Standard 2.0 or higher.

> &gt; dotnet add package HansPeterGit
