using System;
using System.Linq;
using System.Reflection;
using Training.Reflection.LotsOfTypes;
using static Training.Reflection.Demo.ConsoleEx;
namespace Training.Reflection.Demo._03_Attributes
{
    public class Demos
    {
        public static void Entry()
        {
            Console.Clear();
            Console.WriteLine("Category: Attributes");
            Console.WriteLine("================");
            Console.WriteLine();

            Console.WriteLine("1> Marked Attribute");
            Console.WriteLine("2> Assembly Scannning");

            int.TryParse(Console.ReadKey().KeyChar.ToString(), out var choice);
            Console.WriteLine();
            Console.WriteLine();
            switch (choice)
            {
                case 1:
                    MarkedAttribute();
                    break;
                case 2:
                    AssemblyScanning();
                    break;
            }
        }

        public static void MarkedAttribute()
        {
            Console.Clear();
            var type = typeof(UsesAttribute);
            #region one
            var attribute = (IsMarked)type.GetCustomAttribute(typeof(IsMarked));
            Console.WriteLine($"IsMarked with name \"{attribute.Name}\" and Count \"{attribute.Count}\"");
            PressAnyKeyTo();
            #endregion
            
            #region two
            attribute = type.GetCustomAttribute<IsMarked>();
            Console.WriteLine($"IsMarked with name \"{attribute.Name}\" and Count \"{attribute.Count}\"");
            PressAnyKeyTo();
            #endregion
            
        }

        public static void AssemblyScanning()
        {
            Console.Clear();
            var assembly = typeof(MarkerAttribute).Assembly;
            var allTypes = assembly.GetTypes();
            Console.WriteLine($"All types in the assembly {assembly.GetName()}");
            foreach (var type in allTypes)
            {
                Console.WriteLine($"\t{type.Name}");
            }
            
            PressAnyKeyTo();
            
            var markedTypes = allTypes
                .Where(t => t.GetCustomAttribute<MarkerAttribute>() != null);
            Console.WriteLine($"Marked types in the assembly {assembly.GetName()}");
            foreach (var type in markedTypes)
            {
                Console.WriteLine($"\t{type.Name}");
            }
        }
    }
}