// Lab 1.10 : Write an asynchronous C# program using async/await that simulates downloading two files concurrently and prints a message once both downloads are complete.
using System;
using System.Threading.Tasks;

class _10_AsynchronousProgramming
{
    static async Task Main()
    {
        Console.WriteLine("Starting downloads...");

        Task download1 = DownloadFile("File 1", 3000);
        Task download2 = DownloadFile("File 2", 2000);

        await Task.WhenAll(download1, download2);

        Console.WriteLine("Both downloads are complete.");
    }

    static async Task DownloadFile(string fileName, int delay)
    {
        Console.WriteLine(fileName + " download started.");

        await Task.Delay(delay);

        Console.WriteLine(fileName + " download finished.");
    }
}