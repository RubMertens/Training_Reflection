using System;

namespace Training.Reflection.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Categories");
            Console.WriteLine("==========");
            Console.WriteLine("1> Basics");
            Console.WriteLine("2> Generics");
            Console.WriteLine("3> Attributes");
            Console.WriteLine("4> AssemblyLoading");
            Console.WriteLine("5> IL");
            
            int.TryParse(Console.ReadKey().KeyChar.ToString(), out var category);

            switch (category)
            {
                case 1: _01_Basics.Demos.Entry();
                    break;
                case 2: _02_Generics.Demos.Entry();
                    break;
                case 3: _03_Attributes.Demos.Entry();
                    break;
                case 4: _04_AssemblyLoading.Demos.Entry();
                    break;
                case 5: _05_IntermediateLanguage.Demos.Entry();
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("---------------------");
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}