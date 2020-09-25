# Lab 02

## Introduction

In this lab we'll be building a first implementation of the actual mapper that's going to copy properties from objects. 
We will of course be using our previously build `TypeExtensions` to find the relevant `MatchingProperties`

## Walk-through

### Step 1 : Registering Types

First thing we should do is register our mapping in the mapper. 
This can for example be done based on a compound key made from the types passed as generics.

Be careful when choosing what kind of name to use for your key. 
Simply using the `Name` property is probably not enough while the `AssemblyQualifiedName` is probably to complex. 
A good in the middle is using `FullName` this also gives you the best debug feedback.

```c#
private string GetKey(Type FromType, Type ToType)
{
    return $"From_{FromType.FullName}_to_{ToType.FullName}";
}
```

Register the key matchingProperties in a dictionary.

```c#
var matchingProperties = fromType.GetMatchingProperties(toType);
matchingPropertiesByKey.Add(key, matchingProperties);
```

### Step 2 : Using the registeredProperties to copy values

In the `MapTo` method we're now going to use the registered mapping to copy values from one instance to the other.

First make a new instance of the Type we're mapping to using `Activator.CreateInstance`

Next iterate over the matching properties. Getting the value from one object and setting it in the other.

```c#
foreach (var match in matchingProperties)
{
    var value =match.From.GetValue(instance);
    match.To.SetValue(toInstance, value);
}
```

### Step 3 : Add some more validation around the registering and mapping.

Add a guard for double registration to the `Register` method.

```c#
 if(matchingPropertiesByKey.ContainsKey(key))
    throw new InvalidOperationException($"You tried to register a mapping from {fromType.FullName} to {toType.FullName} twice");
```

Add a guard to prevent calling `MapTo` on unregistered types.

```c#
 if(!matchingPropertiesByKey.ContainsKey(key))
    throw new ArgumentException($"No mapping found from {fromType.FullName} to {toType.FullName}");
```