using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.Loader;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sigil;
using Sigil.NonGeneric;

namespace Training.Reflection
{
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
                Count = from.Count
            };
        }
    }

    public class SimpleMapper
    {
        private Dictionary<string, IList<MatchingProperties>>
            matchingPropertiesByKey =
                new Dictionary<string, IList<MatchingProperties>>();

        private Dictionary<string, DynamicMethod> mapperByKey =
            new Dictionary<string, DynamicMethod>();
        
        private Dictionary<string, Delegate> sigilMethodByKey = new Dictionary<string, Delegate>();
        
        private Dictionary<string, Type> mapperTypeByKey = new Dictionary<string, Type>();

        public void Register<TFrom, TTo>() where TTo : new()
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            var key = GetKey(fromType, toType);
            if (matchingPropertiesByKey.ContainsKey(key))
                throw new InvalidOperationException(
                    $"You tried to register a mapping from {fromType.FullName} to {toType.FullName} twice");

            var matchingPropertiesByName = fromType.GetMatchingProperties(toType);
            var matchingPropertiesByAttribute =
                fromType.GetMatchingPropertiesByAttribute(toType);
            var allMatchingProperties = matchingPropertiesByName
                .Union(matchingPropertiesByAttribute).ToList();

            matchingPropertiesByKey.Add(key, allMatchingProperties);

            BuildMapperMethod(fromType, toType);
            BuildMapperWithSigil<TFrom, TTo>();
            BuildRoslynMapper(fromType, toType);
        }

        private void BuildMapperMethod(Type fromType, Type toType)
        {
            var key = GetKey(fromType, toType);
            var dm = new DynamicMethod(
                key,
                toType,
                new[] {fromType}
            );

            /*
.maxstack 3
    .locals init (
      [0] class Training.Reflection.To V_0
    )

    // [22 9 - 22 10]
    IL_0000: nop

    // [23 13 - 26 15]
    IL_0001: newobj       instance void Training.Reflection.To::.ctor()
    IL_0006: dup
    IL_0007: ldarg.0      // from
    IL_0008: callvirt     instance int32 Training.Reflection.From::get_Count()
    IL_000d: callvirt     instance void Training.Reflection.To::set_Count(int32)
    IL_0012: nop
    IL_0013: stloc.0      // V_0
    IL_0014: br.s         IL_0016

    // [27 9 - 27 10]
    IL_0016: ldloc.0      // V_0
    IL_0017: ret
  } // end of method TestCopy::Copy
             */
            var defaultCtor = toType.GetConstructor(Type.EmptyTypes);
            var il = dm.GetILGenerator();
            il.Emit(OpCodes.Newobj, defaultCtor); //Create the new instance
            foreach (var match in matchingPropertiesByKey[key])
            {
                il.Emit(OpCodes.Dup); // duplicate reference on eval stack
                il.Emit(OpCodes.Ldarg_0); //load fromInstance
                il.EmitCall(OpCodes.Callvirt, match.From.GetGetMethod(true),
                    null); //call getter on from and put value on eval stack
                il.EmitCall(OpCodes.Callvirt, match.To.GetSetMethod(true),
                    null); //call setter on duped ref to ToInstance and set value
            } // repeat until all matching properties have been copied!

            il.Emit(OpCodes.Ret); // return the reference to toInstance

            mapperByKey.Add(key, dm);
        }

        private void BuildMapperWithSigil<TFrom, TTo>()
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            var key = GetKey(fromType, toType);
            var emit =Emit<Func<TFrom, TTo>>.NewDynamicMethod();
            
            emit.NewObject(toType);
            foreach (var match in matchingPropertiesByKey[key])
            {
                emit.Duplicate()
                    .LoadArgument(0)
                    .CallVirtual(match.From.GetGetMethod(true))
                    .CallVirtual(match.To.GetSetMethod(true));
            }
            emit.Return();
            var del = emit.CreateDelegate();
            sigilMethodByKey.Add(key, del);
        }
        
            private void BuildRoslynMapper(Type fromType, Type toType)
        {
            var key = GetKey(fromType, toType);
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
            
            var syntaxTree = CSharpSyntaxTree.ParseText(template);
            var compilation = CSharpCompilation.Create(
                $"Reflection.Assembly.SimpleMapper_{Guid.NewGuid()}",
                new[] {syntaxTree},
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using var memoryStream = new MemoryStream();
            var result = compilation.Emit(memoryStream);
            if (!result.Success)
            {
                throw new Exception("Couldn't compile mapper class");
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            
            var assembly = AssemblyLoadContext.Default.LoadFromStream(memoryStream);
            var mapperType = assembly.GetType($"Reflection.Assembly.SimpleMapper.{safeClassName}");
            if (mapperType == null)
                throw new Exception("Couldn't find mapper type");
            
            mapperTypeByKey.Add(key, mapperType);
        }

        private static string GetKey(Type FromType, Type ToType)
        {
            return $"From_{FromType.FullName}_to_{ToType.FullName}";
        }

        public TTo MapTo<TTo>(object instance) where TTo : new()
        {
            var fromType = instance.GetType();
            var toType = typeof(TTo);
            var key = GetKey(fromType, toType);
            if (!matchingPropertiesByKey.ContainsKey(key))
                throw new ArgumentException(
                    $"No mapping found from {fromType.FullName} to {toType.FullName}");


            // var toInstance =
            //     (TTo) mapperByKey[key].Invoke(null, new[] {instance});
            // var toInstance = (TTo) sigilMethodByKey[key].DynamicInvoke(instance);
            var toInstance = (TTo)mapperTypeByKey[key].GetMethod("Map").Invoke(null, new[] {instance});
            
            return toInstance;
        }
    }
}