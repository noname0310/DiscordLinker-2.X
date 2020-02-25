using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace OxideFileIPC
{
    public class DataFileWatcher
    {
        private FileInfo FileInfo;

        private CancellationTokenSource CancellationTokenSource;
        private Task WatcherThread;
        private int FileAccessDelay;

        public DataFileWatcher(string dataDir, string fileName, int accessDelay)
        {
            if (Directory.Exists(dataDir))
            {
                string FilePath = Path.Combine(dataDir, fileName);
                FileInfo = new FileInfo(FilePath);
            }
            else
                throw new Exception("dataFilePath is not vaild!");

            FileAccessDelay = accessDelay;
        }

        public DataFileWatcher(string dataDir, string fileName) : this(dataDir, fileName, 100)
        {

        }

        public Task Start()
        {
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken ct = CancellationTokenSource.Token;

            WatcherThread = Task.Run(() => WatcherRun(ct), ct);
            return WatcherThread;
        }

        public void Stop()
        {
            CancellationTokenSource.Cancel();
        }

        private void WatcherRun(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            while (!ct.IsCancellationRequested)
            {
                FileInfo.Refresh();
                if (FileInfo.Exists)
                {
                    Console.WriteLine("Exist!!");
                }
                else
                    Console.WriteLine("Not Exist");

                Thread.Sleep(FileAccessDelay);
            }
            ct.ThrowIfCancellationRequested();
        }
    }
}
