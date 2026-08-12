using System;
using System.Threading.Tasks;
class _10_AsynchronousProgramming
{
    static async Task Main()
    {
        Console.WriteLine("Download 1 started.");
        await LongTask();
        Console.WriteLine("Download 1 finished.");
    }
    static async Task LongTask()
    {
        Console.WriteLine("Download 2 started.");
        await Task.Delay(3000);
        Console.WriteLine("Download 2 finished.");
    }

}

