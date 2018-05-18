using System;

namespace ConsoleApp1
{
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
}
