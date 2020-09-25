# Lab 04

## Introduction

In this lab we'll convert our relatively slow reflection calls to super fast generated IL. 
We'll do this by defining a `DynamicMethod` which will be used to copy the matching properties.

## Walk-through

### Step 1 : Creating the dynamic method

Make a new method in `SimpleMapper` called `BuildMapperMethod` .

Inside the method instantiate a new `DynamicMethod`

```c#
private void BuildMapperMethod(Type fromType, Type toType)
        {
            var key = GetKey(fromType, toType);
            var matchingProperties = matchingPropertiesByKey[key];
            
            var dm = new DynamicMethod(
                key,
                toType,
                new []{fromType}
                );
        }
```  

### Step 2 : Figure out the exact IL to produce

Create a Dummy class that does what you want. Copy over the properties from one object to another. 
In this example I've chosen to use propertyInitializers, but any technique should work (and result in similar IL, feel free to experiment!)

```c#
  public class From
    {
        public int Count { get; set; }
    }

    public class To
    {
        public int Count { get; set; }
    }
    public class TestCopy
    {
        public static To Copy(From @from)
        {
            return new To()
            {
                Count =  from.Count
            };
        }
    }
```

Now with a tool like ILSpy we can view the generated IL. We'll use this as the base template of our IL.

We can observe IL similar to this one:

```il
.method public hidebysig static 
    class Training.Reflection.To Copy (
        class Training.Reflection.From from
    ) cil managed 
{
    // Method begins at RVA 0x21a0
    // Code size 24 (0x18)
    .maxstack 3
    .locals init (
        [0] class Training.Reflection.To
    )

    // {
    IL_0000: nop
    // 	return new To
    // 	{
    // 		Count = from.Count
    // 	};
    IL_0001: newobj instance void Training.Reflection.To::.ctor()
    IL_0006: dup
    IL_0007: ldarg.0
    IL_0008: callvirt instance int32 Training.Reflection.From::get_Count()
    IL_000d: callvirt instance void Training.Reflection.To::set_Count(int32)
    // (no C# code)
    IL_0012: nop
    IL_0013: stloc.0
    IL_0014: br.s IL_0016

    IL_0016: ldloc.0
    IL_0017: ret
} // end of method TestCopy::Copy
```

### Step 3 Emit the correct il using ILGenerator

Get the IL writer for the method using `GetILGenerator` .

```c#
var il = dm.GetILGenerator();
```

Use the Il Generator to Emit the IL required to copy all the matching properties from one object to another.

If you find it hard to figure out where to put the loop. Try adding some more properties to our Dummy classes and view the IL.

When reading generated IL , you don't have to copy it line for line. Try figuring out what is actually required and what parts are artifacts from the compiler.
You might end up with something like this:

```c#
il.Emit(OpCodes.Newobj, defaultCtor); //Create the new instance
foreach (var match in matchingPropertiesByKey[key])
{
    il.Emit(OpCodes.Dup); // duplicate reference on eval stack
    il.Emit(OpCodes.Ldarg_0); //load fromInstance
    il.EmitCall(OpCodes.Callvirt, match.From.GetGetMethod(true) ,null); //call getter on from and put value on eval stack
    il.EmitCall(OpCodes.Callvirt, match.To.GetSetMethod(true),null); //call setter on duped ref to ToInstance and set value
} // repeat until all matching properties have been copied!
il.Emit(OpCodes.Ret); // return the reference to toInstance
```

### Step 4 : Use the Dynamic method

In our `MapTo` method we should now , instead of using reflection to get and set values, call  our dynamic method.

```c#
var toInstance = (TTo) mapperByKey[key].Invoke(null, new[] {instance});
return toInstance;
```
 
Note we can safely cast to `TTo` here since we know it's the type we registered. Otherwise our method would signature would throw errors.


## Bonus

### TypeBuilder

In some older versions of dotnet you can't use DynamicMethod. However Typebuilder has been around since forever. 
Try building the mapper with a Type instead of a Dynamic Method.

### Safely Build IL

Using `ILGenerator` is rather tricky since it lacks proper ways of debugging or even telling you what went wrong. 
There's a library that does the checking for you which you can try called `Sigil`. 
Try adding a reference to Sigil and using it to generate our dynamic method. It might save you some headaches!