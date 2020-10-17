using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Training.Reflection.Demo._06_Roslyn
{
    public class Demos
    {
        public static void Entry()
        {
            Console.Clear();
            Console.WriteLine("Category: Roslyn");
            Console.WriteLine("===============================");
            Console.WriteLine();

            Console.WriteLine("1> Building Types with Roslyn");

            int.TryParse(Console.ReadKey().KeyChar.ToString(), out var choice);
            Console.WriteLine();
            Console.WriteLine();
            switch (choice)
            {
                case 1:
                    BuildingTypesWithRoslyn();
                    break;
            }
        }

        public static void BuildingTypesWithRoslyn()
        {
            Console.Clear();

            var template =
                @"
namespace Making.With.Roslyn {
    public class MyRoslynBuildClass
    {
        public int Counter { get; set; }
        public string Name;

        public MyRoslynBuildClass(string name)
        {
            Name = name;
        }
    }
}
";
            var refPaths = new[]
            {
                typeof(object).GetTypeInfo().Assembly.Location, // System.Private.CoreLib.dll
                Path.Combine(
                    Path.GetDirectoryName(typeof(GCSettings).GetTypeInfo().Assembly.Location), //System.Runtime.dll
                    "System.Runtime.dll"),
                typeof(Program).Assembly.Location,
            };

            var references = refPaths.Select(path => MetadataReference.CreateFromFile(path)).ToArray();

            var syntaxTree = CSharpSyntaxTree.ParseText(template);
            var compilation = CSharpCompilation.Create($"RoslynRuntimeAssembly", new[] {syntaxTree}, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var memoryStream = new MemoryStream();
            var result = compilation.Emit(memoryStream);
            if (!result.Success)
            {
                throw new Exception("Couldn't compile mapper class");
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            var assembly = AssemblyLoadContext.Default.LoadFromStream(memoryStream);
            var runtimeType = assembly.GetType("Making.With.Roslyn.MyRoslynBuildClass");
            if (runtimeType == null) throw new Exception("Couldn't find type");

            var instance = Activator.CreateInstance(runtimeType, "This is build using Roslyn!");

            Console.WriteLine($"Instance Type: {instance.GetType().FullName}");
            Console.WriteLine($"Name: {instance.GetType().GetField("Name").GetValue(instance)}");
            Console.WriteLine($"Counter: {instance.GetType().GetProperty("Counter").GetGetMethod().Invoke(instance, null)}");
        }
    }
}