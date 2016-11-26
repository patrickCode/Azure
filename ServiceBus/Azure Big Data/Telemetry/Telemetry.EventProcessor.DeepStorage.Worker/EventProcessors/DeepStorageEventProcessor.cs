using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Telemetry.EventProcessor.DeepStorage.Worker.EventProcessors
{
    public class DeepStorageEventProcessor: IEventProcessor
    {
        public async Task OpenAsync(PartitionContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            throw new NotImplementedException();
        }
    }
}
