using System;
using Training.Reflection.Demo._03_Attributes;

namespace Training.Reflection.Demo._03_Attributes
{
    
    public class IsMarked : Attribute
    {
        public string Name { get; }
        public int Count;

        public MetaData MetaData { get; set; }
        
        public IsMarked(string name)
        {
            Name = name;
            MetaData = new MetaData()
            {
                CreatedOn = DateTime.Now
            };
        }
    }

    public class MetaData
    {
        public DateTime CreatedOn { get; set; }
    }
    
    
    public class IsOpenAttribute : Attribute
    {
    }
}