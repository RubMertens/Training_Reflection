using System;
using System.Runtime.CompilerServices;

namespace Training.Reflection.Demo._04_AssemblyLoading
{
    public static class LoadingHelpers
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static WeakReference GetTypesAndUnload(string assemblyLocation)
        {
            var isolatedContext = new UnLoadableAssemblyLoadContext();
            var isolatedAssembly = isolatedContext.LoadFromAssemblyPath(assemblyLocation);
            var types = isolatedAssembly.GetTypes();
            Console.WriteLine($"Amount of types: {types.Length}");
            Console.WriteLine("Press any key to unload the assembly");
            Console.ReadLine();
            var weakRef = new WeakReference(isolatedContext, true);
            isolatedContext.Unload();
            isolatedAssembly = null;
            isolatedContext = null;
            types = null;
            return weakRef;
        }

        public static void WaitUntilUnloaded(WeakReference weakReferenceToLoadContext)
        {
            while (weakReferenceToLoadContext.IsAlive)
            {
                Console.WriteLine("Forcing GC");
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}