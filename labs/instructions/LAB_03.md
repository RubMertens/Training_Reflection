# Lab 03

## Introduction

In this Lab we're going to make our configuration a bit more flexible. We'll add the ability to mark properties with an Attribute that marks what property they should map from and to.

# Walk-through

## Step 1 : Take a look at the MappedFrom Attribute

Add an attribute `MappedFromAttribute` that can only be applied to properties. A name is passed in via the constructor and will later be read via reflection.

<details>
    <summary>show code</summary>

```c#
[AttributeUsage(AttributeTargets.Property)]
    public class MappedFromAttribute: Attribute
``` 
</details>


## Step 2 : Getting properties from types

Add a `AddMatchingPropertiesByAttribute` extension method on type if you're building from scratch.

The first part in this method is the same as the other Type extension. We need to get all the properties from both Types using `GetProperties`. 
Don't forget the "nonpublic" ones.
    
## Step 3 : Filtering based on attribute

Next instead simply of comparing the `Name` attributes we're going to compare the Name of the `self` property with the value of the `MappedFrom` attribute. 

Also check if you can read, write and if the types are the same.
<details>
    <summary>show code</summary>

```c#
var matchingProperties = selfProperties
    .SelectMany(s =>
        otherProperties.Where(
                o => o
                     .GetCustomAttribute<MappedFromAttribute>()
                     ?.Name == s.Name
                     && o.CanRead && o.CanWrite
                     && s.CanRead && s.CanWrite
                     && o.PropertyType == s.PropertyType
                     )
            .Select(o => new MatchingProperties()
            {
                From = s, To = o
            })
    );
```  
</details>


## Step 4 : using matched properties

Lastly we need to use our new matched properties. Add them to the existing matched properties in the `Register` method in `SimpleMapper`

<details>
    <summary>show code</summary>

```c#
var matchingPropertiesByName = fromType.GetMatchingProperties(toType);
var matchingPropertiesByAttribute =
    fromType.GetMatchingPropertiesByAttribute(toType);
var allMatchingProperties = matchingPropertiesByName
    .Union(matchingPropertiesByAttribute).ToList();
```
</details>
