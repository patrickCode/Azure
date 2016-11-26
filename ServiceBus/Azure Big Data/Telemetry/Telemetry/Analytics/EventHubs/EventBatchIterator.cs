using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json.Linq;

namespace Telemetry.Api.Analytics.EventHubs
{   
    //This class is used to create a batch whose size is within the maximum limit (256KB)
    //We are implementing IEnumerator and IEnumerable so that we can iterate over EventData
    public class EventBatchIterator: IEnumerator<IEnumerable<EventData>>, IEnumerable<IEnumerable<EventData>>
    {
        private readonly JArray _allEvents;
        private int _lastBatchedEventIndex;
        private IEnumerable<EventData> _currentBatch;

        public IEnumerable<EventData> Current
        {
            get { return _currentBatch; }
        }
        public static int MaxBatchSizeBytes { get; private set; }
        public static int EventDataOverheadBytes { get; private set; }

        static EventBatchIterator()
        {
            MaxBatchSizeBytes = Int32.Parse(ConfigurationManager.AppSettings["Telemetry.EventHubs.MaxMessageSizeBytes"]);
            EventDataOverheadBytes =
                Int32.Parse(ConfigurationManager.AppSettings["Telemetry.EventHubs.EventDataOverheadBbytes"]);
        }
        public EventBatchIterator(JArray events)
        {
            _allEvents = events;
        }

        
        public void Dispose()
        {
            
        }

        //This is where the next batching starts
        public bool MoveNext()
        {
            var batch = new List<EventData>(_allEvents.Count);
            var batchSize = 0;
            for (var i = _lastBatchedEventIndex; i < _allEvents.Count; i++)
            {
                dynamic evt = _allEvents[i];
                evt.receivedAt = DateTime.UtcNow.ToUnixMilliseconds();
                var payloadSize = 0;
                var eventData = EventDataTransform.ToEventData(evt, out payloadSize);
                var eventSize = payloadSize + EventDataOverheadBytes; //the total size of the event is the message (json) size and the other event properties (metadata that we are sending)
                if (batchSize + eventSize > MaxBatchSizeBytes)
                    break;
                //Add the event to the batch
                batch.Add(eventData);
                batchSize += eventSize;
            }
            _lastBatchedEventIndex += batch.Count;
            _currentBatch = batch;
            return _currentBatch.Any();
        }

        public void Reset()
        {
            _lastBatchedEventIndex = 0;
        }

        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        public IEnumerator<IEnumerable<EventData>> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}