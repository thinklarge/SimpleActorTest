using System;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            Aggregator aggregator = null;
            var router = new Router(Guid.NewGuid(), new ActorFactory(), new ActorInit() { Name = "Workload Actor" }, new Context() { Parent = aggregator });
            aggregator = new Aggregator(Guid.NewGuid(), new Context(), router);
            router.Context.Parent = aggregator;

            aggregator.Recieve(new Workload()
            {
                delayTime = 500,
                isFinished = false,
                size = 1,
                workId = 1
            });

            aggregator.Recieve(new Workload()
            {
                delayTime = 200,
                isFinished = false,
                size = 1,
                workId = 2
            });

            aggregator.Recieve(new Workload()
            {
                delayTime = 500,
                isFinished = false,
                size = 1,
                workId = 3
            });

            aggregator.Recieve(new Workload()
            {
                delayTime = 500,
                isFinished = false,
                size = 1,
                workId = 4
            });

            aggregator.Recieve(new Workload()
            {
                delayTime = 500,
                isFinished = false,
                size = 1,
                workId = 5
            });

            aggregator.Recieve(new Workload()
            {
                delayTime = 500,
                isFinished = false,
                size = 1,
                workId = 6
            });

            aggregator.Recieve(new Workload()
            {
                delayTime = 500,
                isFinished = false,
                size = 1,
                workId = 7
            });

            aggregator.Recieve(new Workload()
            {
                delayTime = 500,
                isFinished = false,
                size = 1,
                workId = 8
            });

            aggregator.Recieve(new Workload()
            {
                delayTime = 500,
                isFinished = false,
                size = 1,
                workId = 1
            });


            await Task.Delay(3000);

            Console.WriteLine(aggregator.total);
        }
    }
}
