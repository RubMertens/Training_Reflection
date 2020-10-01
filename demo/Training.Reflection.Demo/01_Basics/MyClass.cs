namespace Training.Reflection.Demo._01_Basics
{
    public class MyClass
    {
        public int Counter { get; }
        private int SecretCounter { get; }
        public string Name;
        private string _privateName;

        public MyClass()
        {
        }

        public string GetPrivateName()
        {
            return _privateName;
        }
    }
}