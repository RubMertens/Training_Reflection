using System.Reflection;
using System.Runtime.Loader;

namespace Training.Reflection.Demo._04_AssemblyLoading
{
    public class UnLoadableAssemblyLoadContext : AssemblyLoadContext
    {
        public UnLoadableAssemblyLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}