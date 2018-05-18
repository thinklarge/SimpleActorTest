using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class NamedActor : IActor
    {
        private Queue<Func<Task>> tasks;
        public NamedActor(Guid id, string name)
        {
            Id = id;
            Name = name;
            this.tasks = new Queue<Func<Task>>();
        }

        public void Recieve(Workload workloadSize, Guid? workerId = null)
        {
            tasks.Enqueue(
                   async () =>
                   {
                       Console.WriteLine($"worker - {this.Id} workloadId - {workloadSize.workId}");
                       await Task.Delay(workloadSize.delayTime);
                       workloadSize.isFinished = true;

                       this.Context.Router.UpdateWorkload(workloadSize, this.Id);
                   });


            Task.Run(async () => await ExecuteNext()); // TODO: Look into whether or not this will run an infinite chain. 
        }

        private async Task ExecuteNext()
        {
            if (tasks.Count > 0)
            {
                var task = tasks.Dequeue();
                await task();
                await ExecuteNext();
            }
        }

        public string Name { get; }

        public IContext Context { get; set; }

        public Guid Id { get; set; }
    }
}
