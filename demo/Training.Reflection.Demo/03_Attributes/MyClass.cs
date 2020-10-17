using System;
using Training.Reflection.Demo._03_Attributes;


[assembly: IsOpenAttribute]
[module: IsOpenAttribute]
namespace Training.Reflection.Demo._03_Attributes
{
    [IsOpenAttribute]
    public class MyClass
    {
        [IsOpenAttribute]
        [field: IsOpenAttribute]
        public int Count { get; set; }
        [IsOpenAttribute]
        public event EventHandler MyEvent;
        [IsOpenAttribute]
        public string Name;

        [IsOpenAttribute]
        public MyClass()
        {
            
        }

        [IsOpenAttribute]
        [return: IsOpenAttribute]
        public string DoWork()
        {
            return Name;
        }
        
        public T ReturnT<[IsOpenAttribute]T>(T t)
        {
            return t;
        }
    }
}