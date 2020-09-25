# Lab 01

## introduction

In this series of labs we're going to build our very own automapper.

We'll create a mapper that can map properties between types that match name and type.

In the first lab we'll focus on getting the matching properties between two objects.

### Taking a look at the tests

Inside the `Training.Reflection.Tests` project you find a test class with two tests that we want to be green.

Implement the `ObjectExtensions` in the `Training.Reflection` project to find and get the matching properties based on `Type` and `Name`

## Walkthrough

### Step 01 : Getting all relevant properties from the objects 

On both objects we'll need to get the relevant properties. This can be done using the `GetProperties` method on the type.

First get the Type of the object by calling `GetType()` then follow up by getting all properties.

```c#
var selfProperties = self.GetType()
                .GetProperties();
```

This will however only select the public properties by default. In order to get all private ones as well we need to pass in some BindingFlags.

```c#
var selfProperties = self.GetType()
                .GetProperties(
                    BindingFlags.Public
                    | BindingFlags.Instance
                    | BindingFlags.NonPublic
                );
```

Do this for both `self` and `other` .

## Step 02 : Matching the properties.

To make `MatchingProperties` pairs we're going to use some linq to query the two lists and merge them into one.

First we "match" all the properties without filtering them.

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

We of course need to add some more filtering.

Filter out the properties where `Name` and `PropertyType` match

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

`GetMatchingProperties_OnlyReturnsPropertiesWithGettersAndSetters` should still be failing at this point. 
We're not checking whether our properties have actual setter and getters.

Add some more filtering where both self and from's property `CanRead` and `CanWrite`

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