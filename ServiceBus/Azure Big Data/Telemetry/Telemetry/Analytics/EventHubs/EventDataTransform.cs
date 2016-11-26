using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.ServiceBus.Messaging;

namespace Telemetry.Api.Analytics.EventHubs
{
    public class EventDataTransform
    {
        public static EventData ToEventData(dynamic eventObject, out int payloadSize)
        {
            var json = eventObject.ToString();
            payloadSize = Encoding.UTF8.GetByteCount(json);
            var payload = Encoding.UTF8.GetBytes(json);
            var eventData = new EventData(payload)
            {
                PartitionKey = (string) eventObject.deviceId
            };
            eventData.SetEventName((string) eventObject.eventName);
            eventData.SetReceivedAt((long) eventObject.receivedAt);
            return eventData;
        }
    }
}