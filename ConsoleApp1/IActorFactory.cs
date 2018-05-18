namespace ConsoleApp1
{
    public interface IActorFactory<T>
    {
        IActor Create(T initializer, IActor parent, IRouter router);
    }
}
