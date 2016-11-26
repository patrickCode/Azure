using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceBus.Messaging
{
    public static class EventDataExtensions
    {
        public static void SetReceivedAt(this EventData eventData, long receivedAt)
        {
            SetPropertyValue(eventData, "receivedAt", receivedAt);
        }

        public static void SetEventName(this EventData eventData, string eventName)
        {
            SetPropertyValue(eventData, "receivedAt", eventName);
        }

        private static void SetPropertyValue(EventData eventData, string propertyName, object value)
        {
            eventData.Properties[propertyName] = value;
        }
        
    }
}
