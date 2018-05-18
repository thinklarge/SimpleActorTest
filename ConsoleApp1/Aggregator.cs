using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    public class Aggregator : IActor
    {
        private Dictionary<int, Workload> work;
        public int total;

        public Aggregator(Guid id, IContext context, IRouter router)
        {
            Id = id;
            Context = context;
            Router = router;
            this.work = new Dictionary<int, Workload>();
        }

        public void SetRouter(IRouter router)
        {
            this.Router = router;
        }

        public Guid Id { get; set; }

        public IContext Context { get; set; }

        public IRouter Router { get; set; }

        public void Recieve(Workload workload, Guid? workloadId = null)
        {
            if (workloadId == null)
            {
                this.work[workload.workId] = workload;
                this.Router.Recieve(workload, this.Id);
            }
            else
            {
                if (!this.work[workload.workId].isFinished)
                {
                    Console.WriteLine($"Work not finished {workload.workId} {workloadId}");
                    this.work[workload.workId] = workload;
                    total += workload.size * workload.workId;
                }
                else
                {
                    Console.WriteLine($"Work finished {workload.workId} {workloadId}");
                }
            }                             
        }
    }
}
