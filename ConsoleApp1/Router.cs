using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public struct Workload
    {
        public int workId { get; set; }

        public int size { get; set; }

        public int delayTime { get; set; }

        public bool isFinished { get; set; }
    }

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

    public class Context : IContext
    {
        public IActor Parent { get; set; }

        public IRouter Router { get; set; }
    }

    public interface IContext
    {
        IActor Parent { get; set; }

        IRouter Router { get; set; }
    }

    public class Router : IRouter
    {
        private readonly IActorFactory<ActorInit> actorFactory;

        private Queue<IActor> emptyActors;

        private Queue<Workload> workloads;

        public Guid Id { get; set; }

        public Dictionary<Guid, IActor> actors;

        public Router(Guid id, IActorFactory<ActorInit> actorFactory, ActorInit config, IContext context)
        {
            this.Id = id;
            this.actorFactory = actorFactory;
            this.Config = config;
            this.Context = context;
            this.actors = new Dictionary<Guid, IActor>();
            this.emptyActors = new Queue<IActor>();
            this.workloads = new Queue<Workload>();
            this.AddActor();
            this.AddActor();
            this.AddActor();
            this.AddActor();
        }

        public ActorInit Config { get; }

        public IContext Context { get; set; }

        private IActor CreateActor()
        {
            return this.actorFactory.Create(this.Config, this.Context.Parent, this);
        }

        public void AddActor()
        {
            var actor = this.CreateActor();
            actors.Add(actor.Id, actor);
            emptyActors.Enqueue(actor);
        }

        public void RemoveActor()
        {
            if (emptyActors.Any())
            {
                var actor = emptyActors.Dequeue();
                actors.Remove(actor.Id);
            }
            else
            {
                actors.Remove(actors.First().Key);
            }
        }

        public void Setup()
        {
            for (var i = 0; i < 5; i++)
            {
                var actor = CreateActor();
                this.actors[actor.Id] = actor;
                this.emptyActors.Enqueue(actor);
            }
        }

        public void UpdateWorkload(Workload workload, Guid id)
        {
            this.Context.Parent.Recieve(workload, id);
            if (!this.actors.ContainsKey(id))
            {
                return;
            }

            if (this.workloads.Count > 0)
            {
                var work = workloads.Dequeue();                
                this.actors[id].Recieve(work, this.Id);

                if (workload.workId % 4 == 0) // Artificially remove a node
                {
                    this.RemoveActor();
                }
                return;
            }

            this.emptyActors.Enqueue(this.actors[id]);
        }

        public void Recieve(Workload workloadSize, Guid? workloadId = null)
        {

            if (this.emptyActors.Any())
            {
                var actor = this.emptyActors.Dequeue();
                actor.Recieve(workloadSize, actor.Id);
            }
            else
            {
                this.workloads.Enqueue(workloadSize);
            }
        }
    }

    public interface IRouter : IActor
    {
        void UpdateWorkload(Workload workload, Guid id);
    }

    public interface IActorFactory<T>
    {
        IActor Create(T initializer, IActor parent, IRouter router);
    }

    public class ActorInit
    {
        public string Name { get; set; }
    }

    public class ActorFactory : IActorFactory<ActorInit>
    {
        public ActorFactory()
        {
        }

        public IActor Create(ActorInit initializer, IActor parent, IRouter router)
        {
            return new NamedActor(Guid.NewGuid(), initializer.Name)
            {
                Context = new Context()
                {
                    Parent = parent,
                    Router = router
                }
            };
        }
    }


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
