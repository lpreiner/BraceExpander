[![NuGet Package](https://github.com/lpreiner/BraceExpander/actions/workflows/dotnet.yml/badge.svg)](https://github.com/lpreiner/BraceExpander/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/BraceExpander.svg?style=flat&label=nuget&logo=nuget)](https://www.nuget.org/packages/BraceExpander/)

# BraceExpander
A lightweight library for performing brace expansion in .NET

## Getting Started
```ps
Install-Package BraceExpander
```

## Usage Examples

### Basic Usage
```C#
using BraceExpander;

var results = Expander.Expand("part.{01..3}");
// [part.01, part.02, part.03]
 ```

### Numeric Sequence
```C#
var results = Expander.Expand("{1..5}");
// [1, 2, 3, 4, 5]
 ```

### Alpha Sequence
```C#
var results = Expander.Expand("{a..g}");
// [a, b, c, d, e, f, g]
 ```

 ### Custom Increment
```C#
var results = Expander.Expand("{0..10..2}");
// [0, 2, 4, 6, 8, 10]
 ```

### Set Expansion
```C#
var results = Expander.Expand("a{1,2,3}");
// [a1, a2, a3]
 ```

 ### Nested Expansions
```C#
var results = Expander.Expand("{{{{a,b},c,{d..f}},g},h}");
// [a, b, c, d, e, f, g, h]
 ```