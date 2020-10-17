using System;

namespace Training.Reflection.Demo._03_Attributes
{
    [IsMarked("For removal", Count = 12)]
    public class UsesAttribute
    {
         int Counter { get; set; }

        public UsesAttribute()
        {
        }

        public void DoSomething()
        {
        }
    }
}