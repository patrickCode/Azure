using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Diagnostics;
using System.Configuration;

namespace Subscriber
{
    class Program
    {
        static string EventHubName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.EventHub"];
        static string ServiceBusConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        static string StorageName = ConfigurationManager.AppSettings["StorageAccountName"];
        static string StorageKey = ConfigurationManager.AppSettings["StorageAccountKey"];
        static void Main(string[] args)
        {
            AsyncPump.Run(MainAsync);
        }

        static async Task MainAsync()
        {
            var storageConnectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageName, StorageKey);
            var eventProcessorHostName = Guid.NewGuid().ToString();
            var eventProcessorHost = new EventProcessorHost(eventProcessorHostName, EventHubName, EventHubConsumerGroup.DefaultGroupName, ServiceBusConnectionString, storageConnectionString);

            await eventProcessorHost.RegisterEventProcessorAsync<EventProcessor>();

            Console.WriteLine("Reveiving Messages");
            Console.ReadLine();
        }
    }

    class EventProcessor : IEventProcessor
    {
        Stopwatch checkpointStopwatch;

        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine("Processor Shutting Down {0}. Reason - {1}", context.Lease.PartitionId, reason);
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            Console.WriteLine("Event Processor Initiated. Partition - {0}, Offset - {1}", context.Lease.PartitionId, context.Lease.PartitionId);
            this.checkpointStopwatch = new Stopwatch();
            this.checkpointStopwatch.Start();
            return Task.FromResult<object>(null);
        }

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (EventData eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("> {0} (Partition - {1})", data, context.Lease.PartitionId);
            }

            //Call checkpoint every 5 minutes, so that the worker process can resume processing from the 5 minute's back if it restarts.
            if (this.checkpointStopwatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                this.checkpointStopwatch.Restart();
            }
        }
    }
}
