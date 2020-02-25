using System;
using System.Threading.Tasks;
using OxideFileIPC;

namespace DiscordLinker_2.X
{
    class Program
    {
        static void Main(string[] args)
        {
            DataFileWatcher dataFileWatcher = new DataFileWatcher("C:\\noname\\Server\\rustds\\oxide\\data\\DiscordLinker", "DisckrdLinkerData.json");
            Task testtesk = dataFileWatcher.Start();
            testtesk.Wait();
            Console.WriteLine("Hello World!");
        }
    }
}
