using System;
using static Training.Reflection.Demo.ConsoleEx;

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
            Console.Clear();

            #region open generic 
            var openType = typeof(MyGenericClass<>);
            Console.WriteLine("Open Generic Type");
            Console.WriteLine("=================================");
            Console.WriteLine($"IsGeneric:    {openType.IsGenericType}");
            Console.WriteLine($"IsContructed: {openType.IsConstructedGenericType}");
            PressAnyKeyTo();
            #endregion

            #region closed generic
            var closedType = typeof(MyGenericClass<IMyGenericInterface>);
            Console.WriteLine("Closed Generic Type");
            Console.WriteLine("=================================");
            Console.WriteLine($"IsGeneric:    {closedType.IsGenericType}");
            Console.WriteLine($"IsContructed: {closedType.IsConstructedGenericType}");
            Console.WriteLine();
            PressAnyKeyTo();
            #endregion

            #region open generic names
            Console.WriteLine("Open Generic Type");
            Console.WriteLine("=================================");
            Console.WriteLine($"Name:          {openType.Name}");
            Console.WriteLine($"FullName:      {openType.FullName}");
            Console.WriteLine($"ToString:      {openType.ToString()}");
            Console.WriteLine($"QualifiedName: {openType.AssemblyQualifiedName}");
            Console.WriteLine();
            PressAnyKeyTo();
            #endregion

            #region closed generic names
            Console.WriteLine("Closed Generic Type");
            Console.WriteLine("=================================");
            Console.WriteLine($"Name:          {closedType.Name}");
            Console.WriteLine($"FullName:      {closedType.FullName}");
            Console.WriteLine($"ToString:      {closedType.ToString()}");
            Console.WriteLine($"QualifiedName: {closedType.AssemblyQualifiedName}");
            PressAnyKeyTo();
            #endregion

            #region open generic names with mutliple generics

            var multipleOpenType = typeof(MyOtherGenericClass<,>);
            Console.WriteLine("Open Generic Type");
            Console.WriteLine("=================================");
            Console.WriteLine($"Name:          {multipleOpenType.Name}");
            Console.WriteLine($"FullName:      {multipleOpenType.FullName}");
            Console.WriteLine($"ToString:      {multipleOpenType.ToString()}");
            Console.WriteLine($"QualifiedName: {multipleOpenType.AssemblyQualifiedName}");
            Console.WriteLine();
            PressAnyKeyTo();
            #endregion

            #region closed generic names with multiple generics
            var multipleClosedType = typeof(MyOtherGenericClass<IMyGenericInterface, object>);
            Console.WriteLine("Open Generic Type");
            Console.WriteLine("=================================");
            Console.WriteLine($"Name:          {multipleClosedType.Name}");
            Console.WriteLine($"FullName:      {multipleClosedType.FullName}");
            Console.WriteLine($"ToString:      {multipleClosedType.ToString()}");
            Console.WriteLine($"QualifiedName: {multipleClosedType.AssemblyQualifiedName}");
            Console.WriteLine();
            PressAnyKeyTo();

            #endregion
        }

        public static void Constructing()
        {
            Console.Clear();
            var type = typeof(MyGenericClass<>);
            var constructed = type.MakeGenericType(typeof(IMyGenericInterface));
            Console.WriteLine($"type.IsContructed: " + $"{type.IsConstructedGenericType}");
            Console.WriteLine($"constructed.IsContructed: " + $"{constructed.IsConstructedGenericType}");

            var constructedMultiple = typeof(MyOtherGenericClass<,>).MakeGenericType(typeof(object), typeof(object));
            Console.WriteLine($"constructedMultiple: {constructedMultiple.ToString()}");
        }
    }
}