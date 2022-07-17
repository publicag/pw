using System.Collections.Generic;

namespace PWSerwer.Dispatcher
{
    public class QueueItemComparer : IComparer<DispatcherQueueItem> 
    {
        public int Compare(DispatcherQueueItem x, DispatcherQueueItem y)
        {
            if (x.Priority < y.Priority)
                return 1;

            if (x.Priority > y.Priority)
                return -1;

            else
                return 0;
        }
    }
}
