namespace Training.Reflection.Demo._03_Attributes
{
     [IsMarked("For Removal")]
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