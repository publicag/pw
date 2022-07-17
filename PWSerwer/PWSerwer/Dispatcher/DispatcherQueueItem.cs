using PWSerwer.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWSerwer.Dispatcher
{
    public class DispatcherQueueItem
    {
        public TransferFile File { get; }
        public string ClientName { get; }
        public double Priority { get; set; }
        public DateTime InsertionTime { get; }
        public TimeSpan WaitingInQueueTime { get; set; }
        private DispatcherConfig _config;
        public DispatcherQueueItem(TransferFile file, string clientName, DispatcherConfig config)
        {
            File = file;
            ClientName = clientName;
            InsertionTime = DateTime.Now;
            WaitingInQueueTime = TimeSpan.Zero;
            _config = config;
        }

        public void UpdateWaitingTime()
        {
            WaitingInQueueTime = DateTime.Now - InsertionTime + TimeSpan.FromMilliseconds(100);
        }

        public double CalculateProcessingTime()
        {
            return File.Size * _config.ProcessingSpeed;
        }

        public double CalculateFileType()
        {
            if (File.Size == 0)
            {
                return 0;
            }
            else if (File.Size <= _config.SmallFileSize)
            {
                return FileSize.Small;
            }
            else if (File.Size <= _config.MediumFileSize && File.Size > _config.SmallFileSize)
            {
                return FileSize.Medium;
            }
            else
            { 
                return FileSize.Large;
            }
        }

        public void Process(CancellationToken c, ISet<string> processingFiles)
        {
            var clientFileName = $"{ClientName} {File.Name}";
            DispatcherProcessor.RegisterTask(clientFileName, RunAsync(c));

            async Task RunAsync(CancellationToken ct)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[{DateTime.Now}] Przetwarzanie {clientFileName}. Rozmiar: {File.Size}mb. Priorytet: {Priority}");
                while (File.Size > 0)
                {
                    if (ct.IsCancellationRequested)
                    {
                        processingFiles.Remove(clientFileName);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Wstrzymano przetwarzanie pliku {clientFileName}. Pozostało {File.Size}mb do przetworzenia.");
                    }
                    ct.ThrowIfCancellationRequested();
                    if (File.Size < _config.ProcessingSpeed)
                    {
                        double size = File.Size;
                        int waitingTime = Convert.ToInt32(size * 100);
                        File.Size -= size;
                        await Task.Delay(waitingTime).ConfigureAwait(false);
                    }
                    else
                    {
                        File.Size -= _config.ProcessingSpeed;
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                }
            } 
        }
    }
}
