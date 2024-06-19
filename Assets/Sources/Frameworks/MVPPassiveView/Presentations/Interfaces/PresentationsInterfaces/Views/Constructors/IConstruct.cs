namespace Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Views.Constructors
{
    public interface IConstruct<in T>
    {
        void Construct(T playerWallet);
    }
    
    public interface IConstruct<in T, in T2>
    {
        void Construct(T value, T2 value2);
    }
}