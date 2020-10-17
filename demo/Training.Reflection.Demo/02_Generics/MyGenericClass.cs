namespace Training.Reflection.Demo._02_Generics
{
    public interface IMyGenericInterface{}
    
    public class MyGenericClass<TGen> 
        where TGen
        : class, IMyGenericInterface
    {
        
    }
}