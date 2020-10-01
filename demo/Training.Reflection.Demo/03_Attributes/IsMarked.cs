using System;

namespace Training.Reflection.Demo._03_Attributes
{
    public class IsMarked : Attribute
    {
        public string Name;
        public IsMarked(string name)
        {
            Name = name;
        }
    }
    public class IsOpenAttributeAttribute : Attribute
    {
    }
}