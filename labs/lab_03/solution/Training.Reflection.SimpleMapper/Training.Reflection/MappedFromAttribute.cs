using System;

namespace Training.Reflection
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MappedFromAttribute: Attribute
    {
        public string Name;

        public MappedFromAttribute(string name)
        {
            Name = name;
        }
    }
}