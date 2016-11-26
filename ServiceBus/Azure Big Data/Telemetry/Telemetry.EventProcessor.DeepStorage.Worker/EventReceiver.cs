using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Telemetry.EventProcessor.DeepStorage.Worker.EventProcessors;

namespace Telemetry.EventProcessor.DeepStorage.Worker
{
    public class EventReceiver
    {
        private readonly EventProcessorHost _host;

        public EventReceiver()
        {
            //The Host Name (1st paramter) is the name of the physical host (an unique name) that the processor uses to get a lock on the partition.
            //ConsumerGroup is a way of getting events from EventHub into different components
            _host = new EventProcessorHost(Environment.MachineName,
                ConfigurationManager.AppSettings["DeepStorage.EventHubName"],
                ConfigurationManager.AppSettings["DeepStorage.ConsumerGroupName"],
                ConfigurationManager.AppSettings["DeepStorage.InputConnectionString"],
                ConfigurationManager.AppSettings["DeepStorage.CheckpointConnectionString"]);
        }

        //Register the Event processor class where the events will be passed to for processing after they have been received.We are registering the custom class that we have written. It will start host listening on separate threads using the options.
        public async Task RegisterProcessorAsync()
        {
            //We are defining a custom config/options for reading events from Event Hub
            var processorOptions = new EventProcessorOptions
            {
                MaxBatchSize = 5000, //throughtput - how many event that can be loadded in a batch (default - 300)
                PrefetchCount = 1000 //cache - how many events can be pre-loaded while we are processing (default - 300)
            };
            await _host.RegisterEventProcessorAsync<DeepStorageEventProcessor>(processorOptions);
        }
    }
}
