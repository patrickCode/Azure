using System;
using Common.Table;
using Newtonsoft.Json;
using Microsoft.ServiceBus;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Consumer
{
    public class Sender
    {
        private readonly TopicClient _topicClient;
        private readonly TableProvider<StatusTableEntity> _statusTableProvider;
        public Sender(string connectionString, string topicName, TableProvider<StatusTableEntity> statusTableProvider)
        {
            _statusTableProvider = statusTableProvider;
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (namespaceManager.TopicExists(topicName))
            {
                _topicClient = TopicClient.CreateFromConnectionString(connectionString, topicName);
            }
            else
            {
                var topicDescription = new TopicDescription(topicName);
                namespaceManager.CreateTopic(topicDescription);
                _topicClient = TopicClient.CreateFromConnectionString(connectionString, topicName);
            }
        }

        public void SendRequest(ServiceRequest serviceRequest)
        {
            var currentDate = DateTime.UtcNow;
            var json = JsonConvert.SerializeObject(serviceRequest);
            var message = new BrokeredMessage(json)
            {
                CorrelationId = serviceRequest.Type,
                ContentType = "application/json",
            };
            message.Properties.Add("Time", currentDate);
            
            _topicClient.Send(message);

            var status = new StatusInfo()
            {
                Client = serviceRequest.ClientName,
                CreatedTime = currentDate,
                Status = "Created",
                TrackingGuid = serviceRequest.Tracker
            };
            _statusTableProvider.Update(new StatusTableEntity(status));
        }

        public async Task SendRequestAsync(ServiceRequest serviceRequest)
        {
            var currentDate = DateTime.UtcNow;
            var message = new BrokeredMessage(serviceRequest)
            {
                CorrelationId = serviceRequest.Type
            };
            message.Properties.Add("Time", currentDate);
            var senderTask = _topicClient.SendAsync(message);

            var entryTask = Task.Run(() =>
            {
                var status = new StatusInfo()
                {
                    Client = serviceRequest.ClientName,
                    CreatedTime = currentDate,
                    Status = "Created",
                    TrackingGuid = serviceRequest.Tracker
                };
                _statusTableProvider.Update(new StatusTableEntity(status));
            });
            await Task.WhenAll(senderTask, entryTask);
        }
    }
}