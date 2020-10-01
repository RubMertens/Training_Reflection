using System;
using System.Reflection;

namespace Training.Reflection.Demo
{
    public static class TypeExtensions
    {
        public static void DumpFields(this Type type)
        {
            Console.WriteLine();
            Console.WriteLine("Fields:");
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                        BindingFlags.DeclaredOnly);
            foreach (var field in fields)
            {
                Console.WriteLine($"\t{field.Name}");
            }
        }

        public static void DumpProperties(this Type type)
        {
            Console.WriteLine();
            Console.WriteLine("Properties:");
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                                BindingFlags.DeclaredOnly);
            foreach (var property in properties)
            {
                Console.WriteLine($"\t{property.Name}");
            }
        }

        public static void DumpMethods(this Type
            type)
        {
            Console.WriteLine();
            Console.WriteLine("Properties:");
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                          BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                Console.WriteLine($"\t{method.Name}");
            }
        }
    }
}