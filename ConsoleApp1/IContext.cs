namespace ConsoleApp1
{
    public interface IContext
    {
        IActor Parent { get; set; }

        IRouter Router { get; set; }
    }
}
