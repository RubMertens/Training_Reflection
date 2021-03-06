# Lab 01

## Introduction

In this series of labs we're going to build our very own _AutoMapper_.

We'll create a mapper that can map properties between types that match name and type.

In the first lab we'll focus on getting matching properties between two objects.

## Getting started

Each lab provides a start and a solution. Perhaps you want to want to build it from scratch. In that case create a new solution with two projects, a class library and a testing library. These labs will assume you're working with **dotnet core 3.1**. 

The project will be build around some tests defined in the project. If you which you can copy them over.

### Taking a look at the tests

Inside the `Training.Reflection.Tests` project you find a test class with two tests that we want to be green.

Implement the `TypeExtensions` in the `Training.Reflection` project to find and get the matching properties based on `Type` and `Name`

# Walk-through

## Step 01 : Getting all relevant properties from the types 

On both types we'll need to get the relevant properties. This can be done using the `GetProperties` method.

<details>
    <summary>show code </summary>

```c#
var selfProperties = self.GetProperties();
```
</details>

This will however only select the public properties by default. In order to get all private ones as well we need to pass in some BindingFlags.

<details>
    <summary>show code</summary>

```c#
var selfProperties = self
                .GetProperties(
                    BindingFlags.Public
                    | BindingFlags.Instance
                    | BindingFlags.NonPublic
                );
```

</details>

Do this for both `self` and `other` .

## Step 02 : Matching the properties.

To make `MatchingProperties` pairs we're going to use some linq to query the two lists and merge them into one.

First we "match" all the properties without filtering them.

<details>
    <summary>show code</summary>

```c#
var matchingProperties =
    selfProperties
        .SelectMany(s => otherProperties
            .Select(o => 
                new MatchingProperties()
            {
                From = s,
                To = o
            }));
```

</details>


We of course need to add some more filtering.

Filter out the properties where `Name` and `PropertyType` match

<details>
    <summary>show code</summary>

```c#
var matchingProperties =
    selfProperties
        .SelectMany(s => otherProperties
            .Where(o =>
                s.Name == o.Name
                && s.PropertyType == o.PropertyType)
            .Select(o => 
                new MatchingProperties()
            {
                From = s,
                To = o
            }));
```
</details>


`GetMatchingProperties_OnlyReturnsPropertiesWithGettersAndSetters` should still be failing at this point. 
We're not checking whether our properties have actual setter and getters.

Add some more filtering where both self and from's property `CanRead` and `CanWrite`

<details>
    <summary>show code</summary>

```c#
var matchingProperties =
    selfProperties
        .SelectMany(s => otherProperties
            .Where(o =>
                s.Name == o.Name
                && s.PropertyType == o.PropertyType
                && s.CanRead == o.CanRead
                && s.CanWrite == o.CanWrite)
            .Select(o => 
                new MatchingProperties()
            {
                From = s,
                To = o
            }));
```     
</details>
