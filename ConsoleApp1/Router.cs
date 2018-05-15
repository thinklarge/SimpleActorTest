using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Workload
    {

    }

    public class Aggregator : IActor
    {

        public Aggregator(Guid id, IContext context)
        {
            Id = id;
            Context = context;
        }

        public Guid Id { get; set; }

        public IContext Context { get; set; }
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

    public class Router : IRouter,  IActor
    {
        private readonly IActorFactory<ActorInit> actorFactory;

        private Queue<IActor> emptyActors;

        private Queue<int> workloads;

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
            this.workloads = new Queue<int>();
        }

        public ActorInit Config { get; }

        public IContext Context { get; set; }

        public IActor CreateActor()
        {
            return this.actorFactory.Create(this.Config, this.Context.Parent, this.Context.Router);
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

        public void UpdateWorkload(Guid id, int queueSize)
        {
            if (!this.actors.ContainsKey(id))
            {
                throw new IndexOutOfRangeException($"Key of '{id}' not found on Router '{this.Id}'.");
            }

            if (this.workloads.Count > 0)
            {
                this.actors[id].ProcessWorkload(queueSize, this.Id);
                return;
            }

            this.emptyActors.Enqueue(this.actors[id]);
        }

        public void ScheduleWork(int workloadSize)
        {
            var actor = this.emptyActors.Dequeue();

            if (actor == null)
            {
                actor.ProcessWorkload(workloadSize, actor.Id);
            }
            else
            {
                this.workloads.Enqueue(workloadSize);
            }
        }
    }

    public interface IRouter
    {
        void UpdateWorkload(Guid id, int queueSize);
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
        private Queue<Task> tasks;
        public NamedActor(Guid id, string name)
        {
            Id = id;
            Name = name;
            this.tasks = new Queue<Task>();
        }

        public async Task ProcessWorkload(int workloadSize, Guid workloadId)
        {
            tasks.Enqueue(
                new Task(
                    async () =>
                   {
                       Console.WriteLine("starting Process");
                       await Task.Delay(workloadSize);

                       this.Context.Router.UpdateWorkload(workloadId, 0);
                   }));


            await ExecuteNext(); // TODO: Look into whether or not this will run an infinite chain. 
        }

        private async Task ExecuteNext()
        {
            if (tasks.Count > 0)
            {
                var task = tasks.Dequeue();
                await task;
                await ExecuteNext();
            }

            this.Context.Router.UpdateWorkload(this.Id, 0);
        }

        public string Name { get; }

        public IContext Context { get; set; }

        public Guid Id { get; set; }
    }
}
