using System;

namespace ConsoleApp1
{
    public interface IRouter : IActor
    {
        void UpdateWorkload(Workload workload, Guid id);
    }
}
