using System;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus;

namespace Filter.Wiretap
{
    class Program
    {
        private static string _connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        private static string _topicName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.Topic.Name"];

        static void Main(string[] args)
        {
            var subscriptionName = "Wiretap-" + Guid.NewGuid().ToString();
            var subDesc = new SubscriptionDescription(_topicName, subscriptionName);
            var namespaceMgr = NamespaceManager.CreateFromConnectionString(_connectionString);
            namespaceMgr.CreateSubscription(subDesc);

            var subscriptionClient = SubscriptionClient.CreateFromConnectionString(_connectionString, _topicName, subscriptionName);
            subscriptionClient.OnMessage(ProcessMessage);

            Console.WriteLine("Wire tap running ...");
            Console.ReadLine();
        }

        static void ProcessMessage(BrokeredMessage message)
        {
            Console.WriteLine("Message Received");
            foreach(var item in message.Properties)
            {
                Console.Write("{0} - {1}", item.Key, item.Value);
            }
            Console.WriteLine();
        }
    }
}
