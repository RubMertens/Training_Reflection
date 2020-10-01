using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Training.Reflection.Demo._05_IntermediateLanguage
{
    public class Demos
    {
        public static void Entry()
        {
            Console.Clear();
            Console.WriteLine("Category: Generics");
            Console.WriteLine("================");
            Console.WriteLine();

            Console.WriteLine("1> Building Fields");
            Console.WriteLine("2> Building Properties");
            Console.WriteLine("3> Building Backing fields");
            Console.WriteLine("4> Building Getter and Setter");
            Console.WriteLine("5> Fully build Type");

            int.TryParse(Console.ReadKey().KeyChar.ToString(), out var choice);
            Console.WriteLine();
            Console.WriteLine();
            switch (choice)
            {
                case 1: BuildingFields();
                    break;
                case 2: BuildingProperties();
                    break;
                case 3: BuildingBackingFields();
                    break;
                case 4: BuildingGettersAndSetters();
                    break;
                case 5: BuildingTypesFull();
                    break;
            }
        }

        public static void BuildingFields()
        {
            var assemblyName = new AssemblyName("MyRuntimeTypes");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule("MyRunttimeTypes");
            var typeBuilder = module.DefineType("MyTypeBuilderBuildClass");

            //defining fields
            typeBuilder.DefineField("Name", typeof(string), FieldAttributes.Public);

            var type = typeBuilder.CreateType();
            type.DumpFields();
            type.DumpProperties();
            type.DumpMethods();
        }

        public static void BuildingProperties()
        {
            var assemblyName = new AssemblyName("MyRuntimeTypes");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule("MyRunttimeTypes");
            var typeBuilder = module.DefineType("MyTypeBuilderBuildClass");

            //defining fields
            typeBuilder.DefineField("Name", typeof(string), FieldAttributes.Public);

            //defining properties
            var counterPropertyBuilder =
                typeBuilder.DefineProperty("Counter", PropertyAttributes.None, typeof(int), new[] {typeof(int)});
            
            var type = typeBuilder.CreateType();
            type.DumpFields();
            type.DumpProperties();
            type.DumpMethods();
        }

        public static void BuildingBackingFields()
        {
            var assemblyName = new AssemblyName("MyRuntimeTypes");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule("MyRunttimeTypes");
            var typeBuilder = module.DefineType("MyTypeBuilderBuildClass");

            //defining fields
            typeBuilder.DefineField("Name", typeof(string), FieldAttributes.Public);

            //defining properties
            var counterPropertyBuilder =
                typeBuilder.DefineProperty("Counter", PropertyAttributes.None, typeof(int), new[] {typeof(int)});

            //define backing field
            var counterBackingField = typeBuilder.DefineField("_counter", typeof(int), FieldAttributes.Private);

            var type = typeBuilder.CreateType();
            type.DumpFields();
            type.DumpProperties();
            type.DumpMethods();
        }

        public static void BuildingGettersAndSetters()
        {
            var assemblyName = new AssemblyName("MyRuntimeTypes");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule("MyRunttimeTypes");
            var typeBuilder = module.DefineType("MyTypeBuilderBuildClass");

            //defining fields
            typeBuilder.DefineField("Name", typeof(string), FieldAttributes.Public);

            //defining properties
            var counterPropertyBuilder =
                typeBuilder.DefineProperty("Counter", PropertyAttributes.None, typeof(int), new[] {typeof(int)});

            //define backing field
            var counterBackingField = typeBuilder.DefineField("_counter", typeof(int), FieldAttributes.Private);

            //define getter and setter 
            var counterGetMethod = typeBuilder.DefineMethod("get_Counter", MethodAttributes.Public, typeof(int), null);
            var counterSetMethod = typeBuilder.DefineMethod("set_Counter", MethodAttributes.Public, typeof(void),
                new[] {typeof(int)});
            
            var type = typeBuilder.CreateType();
            type.DumpFields();
            type.DumpProperties();
            type.DumpMethods();
            
            Console.WriteLine();
            Console.WriteLine("Press any key to get the Counter Property");
            Console.ReadLine();
            var instance = Activator.CreateInstance(type);
            var counterProp = type.GetProperty("Counter");
            counterProp.GetSetMethod().Invoke(instance, new object?[] {12});
            var count = counterProp.GetGetMethod().Invoke(instance, null);
            Console.WriteLine($"Count: {count}");
        }
        
        
        public static void BuildingTypesFull()
        {
            var assemblyName = new AssemblyName("MyRuntimeTypes");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule("MyRunttimeTypes");
            var typeBuilder = module.DefineType("MyTypeBuilderBuildClass");

            //defining fields
            typeBuilder.DefineField("Name", typeof(string), FieldAttributes.Public);

            //defining properties
            var counterPropertyBuilder =
                typeBuilder.DefineProperty("Counter", PropertyAttributes.None, typeof(int), new[] {typeof(int)});

            //define backing field
            var counterBackingField = typeBuilder.DefineField("_counter", typeof(int), FieldAttributes.Private);

            //define getter and setter 
            var counterGetMethod = typeBuilder.DefineMethod("get_Counter", MethodAttributes.Public, typeof(int), null);
            var counterSetMethod = typeBuilder.DefineMethod("set_Counter", MethodAttributes.Public, typeof(void),
                new[] {typeof(int)});

            //build getter method
            var il = counterGetMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, counterBackingField);
            il.Emit(OpCodes.Ret);

            //build setter method
            il = counterSetMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, counterBackingField);
            il.Emit(OpCodes.Ret);


            counterPropertyBuilder.SetGetMethod(counterGetMethod);
            counterPropertyBuilder.SetSetMethod(counterSetMethod);
            var type = typeBuilder.CreateType();
            var instance = Activator.CreateInstance(type);
            var counterProp = type.GetProperty("Counter");
            counterProp.GetSetMethod().Invoke(instance, new object?[] {12});
            var count = counterProp.GetGetMethod().Invoke(instance, null);
            Console.WriteLine($"Counter: {count}");

            type.DumpFields();
            type.DumpProperties();
            type.DumpMethods();
        }


    }
}