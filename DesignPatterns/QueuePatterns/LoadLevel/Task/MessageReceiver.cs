using System;
using Common.Table;
using Newtonsoft.Json;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Receiver
{
    public class MessageReceiver
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly int _concurrentReceivers;

        public event EventHandler<MessageReceivedArgs> MessageReceived = (sender, args) => { };
        public MessageReceiver(string connectionString, string topicName, string subscriptionName, string correlationFilter, int concurrentReceivers)
        {
            _concurrentReceivers = concurrentReceivers;
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            
            if (!namespaceManager.TopicExists(topicName))
            {
                var topicDescription = new TopicDescription(topicName);
                namespaceManager.CreateTopic(topicDescription);
                TopicClient.CreateFromConnectionString(connectionString, topicName);
            }

            if (namespaceManager.SubscriptionExists(topicName, subscriptionName))
            {
                _subscriptionClient = SubscriptionClient.CreateFromConnectionString(connectionString, topicName, subscriptionName);
            }
            else
            {
                var subscriptionDescription = new SubscriptionDescription(topicName, subscriptionName)
                {
                    DefaultMessageTimeToLive = TimeSpan.FromDays(7),
                    LockDuration = TimeSpan.FromMinutes(2),
                    EnableDeadLetteringOnMessageExpiration = true,
                    EnableDeadLetteringOnFilterEvaluationExceptions = true
                };
                namespaceManager.CreateSubscription(subscriptionDescription, new CorrelationFilter(correlationFilter));
                _subscriptionClient = SubscriptionClient.CreateFromConnectionString(connectionString, topicName, subscriptionName);
            }
        }

        public void Start()
        {
            var onMessageOptions = new OnMessageOptions()
            {
                AutoComplete = false,
                MaxConcurrentCalls = _concurrentReceivers
            };
            _subscriptionClient.OnMessage(message => ProcessMessage(message), onMessageOptions);
        }

        public void Stop()
        {
            _subscriptionClient.Close();
        }

        public void ProcessMessage(BrokeredMessage message)
        {
            ServiceRequest request;
            if (message.ContentType.Equals("application/json"))
            {
                var content = message.GetBody<string>();
                request = JsonConvert.DeserializeObject<ServiceRequest>(content);
            }
            else
            {
                request = message.GetBody<ServiceRequest>();
            }
            var createdTime = (DateTime)message.Properties["Time"];
            var argument = new MessageReceivedArgs()
            {
                Request = request,
                MessageCreatedAt = createdTime
            };
            try
            {
                MessageReceived(this, argument);
                message.Complete();
            }
            catch (Exception exc)
            {
                message.DeadLetter("Processor Failed", exc.ToString());
            }
        }
    }
}