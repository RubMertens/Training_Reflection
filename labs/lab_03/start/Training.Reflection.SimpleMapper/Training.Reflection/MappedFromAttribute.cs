using System;

namespace Training.Reflection
{
    public class MappedFromAttribute: Attribute
    {
        public string Name;

        public MappedFromAttribute(string name)
        {
            Name = name;
        }
    }
}