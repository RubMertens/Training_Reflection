using System;

namespace Training.Reflection.Demo
{
    public static class ConsoleEx
    {
        public static void PressAnyKeyTo(string doSomething = "continue")
        {
            Console.WriteLine($"Press any key to {doSomething}");
            Console.ReadLine();
        }
    }
}