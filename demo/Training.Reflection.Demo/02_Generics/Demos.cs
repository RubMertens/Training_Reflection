using System;

namespace Training.Reflection.Demo._02_Generics
{
    public class Demos
    {
        public static void Entry()
        {
            Console.Clear();
            Console.WriteLine("Category: Generics");
            Console.WriteLine("================");
            Console.WriteLine();

            Console.WriteLine("1> Generic Names");
            Console.WriteLine("2> Constructing generics");

            int.TryParse(Console.ReadKey().KeyChar.ToString(), out var choice);
            Console.WriteLine();
            Console.WriteLine();
            switch (choice)
            {
                case 1:
                    GenericsNames();
                    break;
                case 2:
                    Constructing();
                    break;
            }
        }

        public static void GenericsNames()
        {
            var type = typeof(MyGenericClass<>);
            Console.WriteLine("Open Generic Type");
            Console.WriteLine("=================================");
            Console.WriteLine($"IsGeneric:    {type.IsGenericType}");
            Console.WriteLine($"IsContructed: {type.IsConstructedGenericType}");
            var closedType = typeof(MyGenericClass<IMyGenericInterface>);
            Console.WriteLine("Closed Generic Type");
            Console.WriteLine("=================================");
            Console.WriteLine($"IsGeneric:    {closedType.IsGenericType}");
            Console.WriteLine($"IsContructed: {closedType.IsConstructedGenericType}");
            Console.WriteLine();
            Console.WriteLine("Open Generic Type");
            Console.WriteLine("=================================");
            Console.WriteLine($"Name:          {type.Name}");
            Console.WriteLine($"FullName:      {type.FullName}");
            Console.WriteLine($"ToString:      {type.ToString()}");
            Console.WriteLine($"QualifiedName: {type.AssemblyQualifiedName}");
            Console.WriteLine();
            Console.WriteLine("Closed Generic Type");
            Console.WriteLine("=================================");
            Console.WriteLine($"Name:          {closedType.Name}");
            Console.WriteLine($"FullName:      {closedType.FullName}");
            Console.WriteLine($"ToString:      {closedType.ToString()}");
            Console.WriteLine($"QualifiedName: {closedType.AssemblyQualifiedName}");
        }

        public static void Constructing()
        {
            var type = typeof(MyGenericClass<>);
            var constructed = type.MakeGenericType(typeof(IMyGenericInterface));
            Console.WriteLine($"type.IsContructed: " + $"{type.IsConstructedGenericType}");
            Console.WriteLine($"constructed.IsContructed: " + $"{constructed.IsConstructedGenericType}");
        }
    }
}