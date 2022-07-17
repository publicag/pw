using PWSerwer.Client;
using PWSerwer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWSerwer.Dispatcher
{
    public class DispatcherProcessor
    {
        ConcurrentDictionary<string, IClient> _client;
        private DispatcherProcessor()
        {
            DispatcherConfig config = null;
            Console.WriteLine("Plik konfiguracyjny:");
            var json = Console.ReadLine();

            try
            {
                config = DispatcherConfig.Deserialize(json);
            }
            catch (Exception)
            {
                Console.WriteLine("Błędny plik konfiguracyjny. Zastosowano domyślną konfigurację.");
                Console.WriteLine("Mały plik: 10. Średni plik: 50. Szybkość przetwarzania 2.");
            }


            if (config != null)
            {
                _dispatcherQueue = new DispatcherQueue<DispatcherQueueItem>(config);
            }
            else
            {
                _dispatcherQueue = new DispatcherQueue<DispatcherQueueItem>(new DispatcherConfig());
            }

            _processingFiles = new HashSet<string>();
            tokenSource = new CancellationTokenSource();
            cancellation = tokenSource.Token;
        }

        private static readonly Lazy<DispatcherProcessor> lazy = new Lazy<DispatcherProcessor>(() => new DispatcherProcessor());

        private DispatcherQueue<DispatcherQueueItem> _dispatcherQueue;
        private HashSet<string> _processingFiles;
        private static readonly Dictionary<Task, string> _listOfThreads = new Dictionary<Task, string>();
        private static CancellationToken cancellation;
        private static CancellationTokenSource tokenSource;

        public static DispatcherProcessor Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public void AddToQueue(ICollection<TransferFile> files, string clientName)
        {
            foreach (var file in files)
                _dispatcherQueue.AddToQ(file, clientName);
            _dispatcherQueue.RecalulateAndSort();
        }

        public async void Process(ConcurrentDictionary<string, IClient> client)
        {
            _client = client;
            if (_listOfThreads.Count != 0)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                tokenSource = new CancellationTokenSource();
                cancellation = tokenSource.Token;
                _listOfThreads.Clear();
            }
            AddToProcessingQueue();
            await WaitForAllOpenTasksToComplete();
        }

        public void AddToProcessingQueue()
        {
            int queueCount = _dispatcherQueue.Count;

            if (queueCount > 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    _dispatcherQueue.AddNextNotIn(_processingFiles, cancellation);
                }
            }
            else
            {
                for (int i = 0; i < queueCount; i++)
                {
                    _dispatcherQueue.AddNextNotIn(_processingFiles, cancellation);
                }
            }
        }

        public static void RegisterTask(string description, Task task)
        {
            lock (_listOfThreads)
            {
                if (!_listOfThreads.ContainsKey(task))
                {
                    _listOfThreads.Add(task, description);
                }
            }
        }

        private async Task WaitForAllOpenTasksToComplete()
        {
            try
            {
                while (true)
                {
                    var tasks = new List<Task>();
                    lock (_listOfThreads)
                    {
                        if (_listOfThreads.Count == 0)
                        {
                            return;
                        }

                        tasks.AddRange(_listOfThreads.Keys);
                    }

                    var task = await Task.WhenAny(tasks).ConfigureAwait(false);

                    lock (_listOfThreads)
                    {
                        if (_listOfThreads.ContainsKey(task))
                        {
                            IClient currentClient;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[{DateTime.Now}] {_listOfThreads[task]} przetworzony.");
                            _client.TryGetValue(_listOfThreads[task].Split(' ')[0], out currentClient);
                            currentClient.FileProcessed($"[{DateTime.Now}] {_listOfThreads[task]} przetworzony.");
                            _listOfThreads.Remove(task);
                            _dispatcherQueue.AddNextNotIn(_processingFiles, cancellation);
                        }
                    }

                    await task.ConfigureAwait(false);

                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
