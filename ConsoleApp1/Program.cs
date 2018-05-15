using System;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var smallArray = new int[] { 1, 3, 6, 8, 9 };
            var largeArray = new int[] { 2, 4, 5, 7, 11, 12 };
            var search = new BinarySearch();
            Console.WriteLine(search.TwoArray(smallArray, largeArray));

            var sub = new SubString();
            Console.WriteLine($"Max = {sub.LengthOfLongestSubstring("dvdf")}");
            Console.WriteLine("Hello World!");
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            var aggregator = new Aggregator(Guid.NewGuid(), new Context());
            var list = await bs.GetList();
        }
    }
}
