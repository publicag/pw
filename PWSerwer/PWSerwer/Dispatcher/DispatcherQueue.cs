using PWSerwer.Models;
using System.Collections.Generic;
using System.Threading;

namespace PWSerwer.Dispatcher
{
    public class DispatcherQueue<TFile> : List<TFile> where TFile : DispatcherQueueItem
    {
        private DispatcherConfig _config;

        public DispatcherQueue(DispatcherConfig config)
        {
            _config = config;
        }

        public void RecalculateQueuePriority()
        {
            var weight = CalculateTotalWeight();

            foreach (TFile item in this)
            {
                item.UpdateWaitingTime();
                CalculatePriority(item, weight);
            }
        }

        public double CalculateTotalWeight()
        {
            double weight = 0;

            foreach (TFile item in this)
            {
                weight += item.File.Size;
            }

            return weight;
        }

        public void CalculatePriority(TFile file, double totalWeight)
        {
            file.Priority = (file.WaitingInQueueTime.TotalSeconds / file.CalculateProcessingTime()) * (1 - file.File.Size / totalWeight) * file.CalculateFileType();
        }

        public void SortByPriority()
        {
            Sort(new QueueItemComparer());
        }

        public void AddToQ(TransferFile file, string clientName)
        {
            var queueItem = new DispatcherQueueItem(file, clientName, _config);
            Add((TFile)queueItem);
        }

        public void RecalulateAndSort()
        {
            RecalculateQueuePriority();
            SortByPriority();
        }

        public void AddNextNotIn(ISet<string> processingFiles, CancellationToken ct)
        {
            foreach (TFile item in this)
            {
                var clientFileName = $"{item.ClientName} {item.File.Name}";
                if (!processingFiles.Contains(clientFileName))
                {
                    item.Process(ct, processingFiles);
                    processingFiles.Add(clientFileName);
                    break;
                }
            }
        }
    }
}
