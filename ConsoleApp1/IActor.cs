using System;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public interface IActor
    {
        Guid Id { get; set; }

        IContext Context { get; set; }

        Task ProcessWorkload(int workloadSize, Guid workloadId);
    }
}