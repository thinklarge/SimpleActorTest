namespace ConsoleApp1
{
    public class Context : IContext
    {
        public IActor Parent { get; set; }

        public IRouter Router { get; set; }
    }
}
