using System;
using System.CommandLine;
using System.Threading.Tasks;
using kiota.Handlers;

namespace kiota
{
    static class Program
    {
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("Debug!!!");
            await new KiotaGenerationCommandHandler().Invoke();
            Console.WriteLine("Done???");
            return 0;
        }
    }
}
