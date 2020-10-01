using System;
using Training.Reflection.LotsOfTypes;

namespace Training.Reflection.Demo._04_AssemblyLoading
{
    public class Demos
    {
        public static void Entry()
        {
            Console.Clear();
            Console.WriteLine("Category: AssemblyLoading");
            Console.WriteLine("================");
            Console.WriteLine();

            Console.WriteLine("1> Loading Types Memory Issue");
            Console.WriteLine("2> Loading Types Memory fix");

            int.TryParse(Console.ReadKey().KeyChar.ToString(), out var choice);
            Console.WriteLine();
            Console.WriteLine();
            switch (choice)
            {
                case 1:
                    TypesInMemProblem();
                    break;
                case 2:
                    TypesInMemSolution();
                    break;
            }
        }
        
        public static void TypesInMemProblem()
        {
            Console.WriteLine("Press any key to scan the assembly for types");
            Console.ReadLine();
            var types = typeof(MarkerAttribute).Assembly.GetTypes();
            Console.WriteLine($"Amount of types: {types.Length}");
            Console.WriteLine("Press any key to remove root reference");
            Console.ReadLine();
            types = new Type[0];
            Console.WriteLine("Press any key to force GC.Collect");
            Console.ReadLine();
            GC.Collect();
        }

        public static void TypesInMemSolution()
        {
            Console.WriteLine("Press any key to reload assembly");
            Console.ReadLine();
            var assemblyLocation = typeof(MarkerAttribute).Assembly.Location;
            var weakReferenceToLoadContext = LoadingHelpers.GetTypesAndUnload(assemblyLocation);
            LoadingHelpers.WaitUntilUnloaded(weakReferenceToLoadContext);
            Console.WriteLine("Unloaded assembly");
        }
    }
}