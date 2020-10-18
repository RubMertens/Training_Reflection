# Lab 05

## Introduction

In this lab we're going for the easiest solution to making runtime types. Roslyn. 
Using Roslyn code generation we'll recreate our IL implementation which should still run at a respectable speed, but be a lot nicer to maintain!

# Walk-through

## Step 01 : Adding package references

 Add a reference to `Microsoft.CodeAnalysis.CSharp` using you're favourite tool. 

 ## Step 02 : Building string template

 With Roslyn we're essentially using c#'s compiler at runtime. The compiler works with strings that represent code. First thing we're going to need is  a string representation of the class we're trying to build.

Add a method `BuildRoslynMapper` to the `SimpleMapper` class. inside this method make a template string which represents the class you're trying to make at runtime.

<details>
    <summary>show code</summary>

```c#
            var template = $@"
namespace Reflection.Assembly.SimpleMapper {{
    public class Copy {{
        public static ToType Map(FromType instance){{
            return new ToType()
            {{
                //map properties    
            }}
        }}
    }}
}}
";
```
</details>

Next let's make it dynamic. Use the names in `fromType` and `toType` to build a _mostly_ unique name. Use string interpolation to replace the variable bits in the template string.

<details>
    <summary>show code</summary>`
    
```c#
var key = GetKey(fromType, toType);
var safeClassName = $"Copy_{key.Replace(".", "_")}"; 
var template = $@"
namespace Reflection.Assembly.SimpleMapper {{
    public class {safeClassName} {{
        public static {toType.FullName} Map({fromType.FullName} instance){{
            return new {toType.FullName}()
            {{
                //map properties    
            }}
        }}
    }}
}}
";
```
</details>

We want our properties to map according to the matching properties.

Get the matching properties from the existing dictionary and iterate over them, mapping properties from the `fromType` to the `toType` like you would write it in c# `ToPropertyName = instance.FromPropertyName`

<details>
    <summary>show code</summary>

```c#

var template = $@"
namespace Reflection.Assembly.SimpleMapper {{
    public class {safeClassName} {{
        public static {toType.FullName} Map({fromType.FullName} instance){{
            return new {toType.FullName}()
            {{
            {string.Join(",\n", matchingPropertiesByKey[key]
                .Select(match => $"{match.To.Name} = instance.{match.From.Name}"))}
            }}
        }}
    }}
}}
";
```
</details>

## Step 03 : Compiling the code

We won't go super in depth here but the general idea is that you need to parse the template string into a `SyntaxTree` which then can be compiled into functioning C#.

Make a `SyntaxTree` from the template using the static class `CSharpSyntaxTree`.

<details>
    <summary>show code</summary>

```c#
   SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(template);
```
</details>

Before we can  call `CSharpCompilation.Create` to compile our Syntax tree we need some more runtime data. Namely what references should be linked to this newly created dll. 

We'll definitely need references for both `fromType` and `toType` as they might come from entirely separate assemblies.

We'll also need references to the runtime and to the CLR.
Those can be acquired with via following types:

```c#
 typeof(object).GetTypeInfo().Assembly.Location;
Path.Combine(Path.GetDirectoryName(typeof(GCSettings).GetTypeInfo().Assembly.Location),"System.Runtime.dll");
```

Gather all required paths and use `MetadataReference.CreateFromFile` to make them into MetadataReferences.

<details>
    <summary>show code</summary>

```c#
var refPaths = new[]
{
    typeof(object).GetTypeInfo().Assembly.Location,
    Path.Combine(
        Path.GetDirectoryName(typeof(GCSettings).GetTypeInfo()
            .Assembly.Location),
        "System.Runtime.dll"),
    this.GetType().Assembly.Location,
    fromType.Assembly.Location,
    toType.Assembly.Location
};

var references = refPaths
    .Select(path => MetadataReference.CreateFromFile(path)).ToArray();
```
</details>

Now we can call `CSharpCompilation.Create` to create a compilation object

<details>
    <summary>show code</summary>

```c#
var compilation = CSharpCompilation.Create(
                $"Reflection.Assembly.SimpleMapper_{Guid.NewGuid()}",
                new[] {syntaxTree},
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );
```
</details>

`CSharpCompilation` allows us compile into a `Stream`. You could this to write your code to disk , or in our case have it compile to a `MemoryStream` for immediate consumption. 

The compilation returns a result which contain **normal c# errors** that can be used to verify your mistakes and typo's.

<details>
    <summary>show code</summary>

```c#
using var memoryStream = new MemoryStream();
var result = compilation.Emit(memoryStream);
if (!result.Success)
{
    throw new Exception("Couldn't compile mapper class");
}
```
</details>


You should now be able to load the dynamic assembly by calling the `AssemblyLoadContext.Default.LoadFromStream`.

Load in the type using `assembly.GetType`.

And finally add the type to a Dictionary in the `SimpleMapper` class.

<details>
    <summary>show code</summary>

```c#
memoryStream.Seek(0, SeekOrigin.Begin);

var assembly = AssemblyLoadContext.Default.LoadFromStream(memoryStream);
var mapperType = assembly.GetType($"Reflection.Assembly.SimpleMapper.{safeClassName}");
if (mapperType == null)
    throw new Exception("Couldn't find mapper type");

mapperTypeByKey.Add(key, mapperType);
```
</details>

## Step 04 : Using the new mapper

Call the new Build method in `Register`.

In the `MapTo` method we're now going to switch our implementation to Roslyn.

Instead of getting the dynamicmethod, get the type you added to the Dictionary get the correct method `Map` and call it. 

It's a static method so it's not bound to an instance.

<details>
    <summary>show code</summary>

```c#
var toInstance = (TTo)mapperTypeByKey[key].GetMethod("Map").Invoke(null, new[] {instance});
```
</details>

## Step 05 : Fixing some mistakes

When running the tests you notice most of them fail. It appears our template is not compiler proof!

Our test classes are nested. Which in reflection is represented with the '+' symbol. A symbol that is illegal for c# syntax.

Sanitize the dirty strings so the template contains only valid c#.

<details>
    <summary>show code</summary>

```c#
var safeClassName = $"Copy_{key.Replace(".", "_").Replace("+", "_")}"; 
            var template = $@"
namespace Reflection.Assembly.SimpleMapper {{
public class {safeClassName}
    {{
        public static {toType.FullName.Replace("+",".")} Map({fromType.FullName.Replace("+",".")} instance)
        {{
            return new {toType.FullName.Replace("+",".")}()
            {{
                {string.Join(",\n", matchingPropertiesByKey[key]
                .Select(match => $"{match.To.Name} = instance.{match.From.Name}"))}
                   
            }};
        }}
    }}
}}
";
```
<details>


Some tests are still failing. When using Roslyn we're also bound by the same rules as c#. This means this implementation can't meddle with private members. Ignore those two tests in the test suite. 