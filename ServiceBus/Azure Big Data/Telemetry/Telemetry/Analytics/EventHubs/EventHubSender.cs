using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telemetry.Api.Analytics.Spec;

namespace Telemetry.Api.Analytics.EventHubs
{
    public class EventHubSender: IEventSender
    {
        public async Task SendEventsAsync(JArray events, string deviceId)
        {
            var connectionString = ConfigurationManager.AppSettings["Telemetry.DeviceEvents.ConnectionString"];
            var eventHubName = ConfigurationManager.AppSettings["Telemetry.DeviceEvents.EventHubName"];
            var client = EventHubClient.CreateFromConnectionString(connectionString, eventHubName);

            //This is used to send the events 1 at a time. Its costly and a lot of calls are made to the Event hub. A TCP connection needs to be kept open until all the events has been sent to the hub.
            //foreach (dynamic eventObject in events)
            //{
            //    eventObject.receivedAt = DateTime.UtcNow.Millisecond;
            //    var json = eventObject.ToString(Formatting.None);
            //    var eventData = new EventData(Encoding.UTF8.GetBytes(json))
            //    {
            //        PartitionKey = deviceId
            //    };
            //    eventData.SetEventName((string) eventObject.eventName);
            //    eventData.SetReceivedAt((long) eventObject.receivedAt);
            //    await client.SendAsync(eventData);
            //}

            //This can be improved by getting all the batches from the iterator and then sending them together as async calls, rather that awaiting for each batch
            var iterator = new EventBatchIterator(events);
            foreach (var batch in iterator)
            {
                await client.SendBatchAsync(batch);
            }
        }
    }
}

/*
 * Limitations of Sending Events in a Batch
 *  - Message size cannot exceed 256 KB
 *  - All the batched events must have a same Partition Key. So if we try to batch events coming from different sources, they will have different device IDs, hence we won't be able to batch them.
*/