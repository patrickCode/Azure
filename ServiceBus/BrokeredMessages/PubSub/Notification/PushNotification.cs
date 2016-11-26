using System;
using Microsoft.ServiceBus;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace Notification
{
    public class PushNotification
    {
        SubscriptionClient subscriptionClient;
        public PushNotification()
        {
            var subscriptionName = Guid.NewGuid().ToString();
            var namespaceMgr = NamespaceManager.CreateFromConnectionString(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"]);

            var subscriptionDescription = new SubscriptionDescription(ConfigurationManager.AppSettings["Microsoft.ServiceBus.Topic.Name"], subscriptionName)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
            };
            namespaceMgr.CreateSubscription(subscriptionDescription);

            subscriptionClient = SubscriptionClient.CreateFromConnectionString(
                ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"],
                ConfigurationManager.AppSettings["Microsoft.ServiceBus.Topic.Name"],
                subscriptionName);

            var onMessageOptions = new OnMessageOptions()
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            subscriptionClient.OnMessage(message => ProcessMessage(message), onMessageOptions);
        }

        private void ProcessMessage(BrokeredMessage message)
        {
            if (message.ContentType.ToLower().Equals("text"))
            {
                Console.WriteLine("Message Received - " + message.GetBody<string>());
            }
            message.Complete();
        }

        private void StopListening()
        {
            subscriptionClient.Close();
        }
    }
}