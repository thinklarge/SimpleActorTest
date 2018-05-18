using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
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
}
