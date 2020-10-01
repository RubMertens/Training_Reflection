using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Training.Reflection.Demo._01_Basics
{
    public class Demos
    {
        public static void Entry()
        {
            Console.Clear();
            Console.WriteLine("Category: Basics");
            Console.WriteLine("================");
            Console.WriteLine();

            Console.WriteLine("1> Getting 'Type'");
            Console.WriteLine("2> Members by BindingFlags");
            Console.WriteLine("3> Getting values with reflection");
            Console.WriteLine("4> Setting values with reflection");
            Console.WriteLine("5> Setting private values");
            Console.WriteLine("6> BypassingProperty settters");
            Console.WriteLine("7> Creating instances");

            int.TryParse(Console.ReadKey().KeyChar.ToString(), out var choice);
            Console.WriteLine();
            Console.WriteLine();
            switch (choice)
            {
                case 1 : GetTypes();
                    break;
                case 2: MembersByBindingFlags();
                    break;
                case 3: GettingValues();
                    break;
                case 4: SettingValues();
                    break;
                case 5: SettingPrivateValues();
                    break;
                case 6: BypassingProperties();
                    break;
                case 7: CreatingInstances();
                    break;
            }

        }
        
        public static void GetTypes()
        {
            var typeFromContext = typeof(MyClass);
            var typeFromRuntime = new MyClass().GetType();

            var castToObject = (object) new MyClass();
            var type = castToObject.GetType();
            Console.WriteLine("Type is: ", type.FullName);
        }
        
        public static void MembersByBindingFlags()
        {
            var members = typeof(MyClass).GetMembers();
            PrintMembers(members);

            Console.WriteLine();
            Console.WriteLine($"Try to find members with {nameof(BindingFlags.NonPublic)}");
            PressAnyKeyTo("print NonPublic members");
            members = typeof(MyClass).GetMembers(BindingFlags.NonPublic);
            PrintMembers(members);

            Console.WriteLine("Try to find private members of the instance.");
            members = typeof(MyClass).GetMembers(
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            PrintMembers(members);


            Console.WriteLine();
            Console.WriteLine("Try to find only members of the declared type.");
            PressAnyKeyTo("print declared private instance members");

            members = typeof(MyClass).GetMembers(
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly
            );
            PrintMembers(members);
        }

        
        private static void PressAnyKeyTo(string doSomething)
        {
            Console.WriteLine($"Press any key to {doSomething}");
            Console.ReadLine();
        }

        private static void PrintMembers(IEnumerable<MemberInfo> members)
        {
            Console.WriteLine();
            foreach (var group in members.GroupBy(m => m.MemberType))
            {
                Console.WriteLine($"{group.Key}");
                Console.WriteLine("=====================");
                foreach (var m in group)
                {
                    Console.WriteLine($"\t{m}");
                }
            }
        }

        public static void GettingValues()
        {
            var instance = new MyClass();
            var field = instance.GetType().GetField(nameof(MyClass.Name));
            var value = field.GetValue(instance);
            Console.WriteLine($"Field value: \"{value}\"");
        }

        public static void SettingValues()
        {
            var instance = new MyClass();
            var field = instance.GetType().GetField(nameof(MyClass.Name));
            Console.WriteLine($"Field value: \"{instance.Name}\"");
            field.SetValue(instance, "New value!");
            Console.WriteLine($"Field value: \"{instance.Name}\"");
        }

        public static void SettingPrivateValues()
        {
            var instance = new MyClass();
            var field = instance.GetType().GetField("_privateName", BindingFlags.NonPublic | BindingFlags.Instance);
            Console.WriteLine($"Field value: \"{instance.GetPrivateName()}\"");
            field.SetValue(instance, "New value!");
            Console.WriteLine($"Field value: \"{instance.GetPrivateName()}\"");
        }

        public static void BypassingProperties()
        {
            var instance = new MyClass();
            var field = instance.GetType()
                .GetField("<Counter>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            Console.WriteLine($"Field value: \"{instance.Counter}\"");
            field.SetValue(instance, 13);
            Console.WriteLine($"Field value: \"{instance.Counter}\"");
        }

        public static void CreatingInstances()
        {
            //creating with CreateInstance returns type object
            var instance = Activator.CreateInstance(typeof(MyClass));
            var type = instance.GetType();
            Console.WriteLine($"Instance of type: {type.Name}");
            //creating with CreateInstance`T returns type T
            var instanceWithGeneric = Activator.CreateInstance<MyClass>();

            var personWithName = (PersonWithName) Activator
                .CreateInstance(typeof(PersonWithName), "My name is Ruben");
            type = instance.GetType();
            Console.WriteLine("Creating via Activator");
            Console.WriteLine($"Instance of type: {type.Name} " + $"with value {personWithName.Name}");

            var ctor = typeof(PersonWithName)
                .GetConstructor(new[] {typeof(string)});
            personWithName = (PersonWithName) ctor.Invoke(new[] {"My Name is Tim"});
            Console.WriteLine("Creating via Constructor");
            Console.WriteLine($"Value = {personWithName.Name}");
        }

    }
}