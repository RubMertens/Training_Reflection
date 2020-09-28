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
            BuildRoslynMapper(fromType, toType);
        }

        private void BuildRoslynMapper(Type fromType, Type toType)
        {
            var key = GetKey(fromType, toType);
            var template = $@"
namespace Reflection.Assembly.SimpleMapper {{
public class Copy_{key.Replace(".", "_")}
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
            var mapperType = assembly.GetType($"Reflection.Assembly.SimpleMapper.Copy_{key.Replace(".","_")}");
            if (mapperType == null)
                throw new Exception("Couldn't find mapper type");
            
            mapperTypeByKey.Add(key, mapperType);
        }

        private static string GetKey(Type FromType, Type ToType)
        {
            return $"From_{FromType.FullName.Replace("+", "_")}_to_{ToType.FullName.Replace("+", "_")}";
        }

        public TTo MapTo<TTo>(object instance) where TTo : new()
        {
            var fromType = instance.GetType();
            var toType = typeof(TTo);
            var key = GetKey(fromType, toType);
            if (!matchingPropertiesByKey.ContainsKey(key))
                throw new ArgumentException(
                    $"No mapping found from {fromType.FullName} to {toType.FullName}");

            var mapperType = mapperTypeByKey[key];
            var toInstance = (TTo) mapperType.GetMethod("Map").Invoke(null, new[] {instance});
            return toInstance;
        }
    }
}