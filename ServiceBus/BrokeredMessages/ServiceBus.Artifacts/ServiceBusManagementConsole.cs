using System;
using Microsoft.ServiceBus;
using System.Collections.Generic;
using Microsoft.ServiceBus.Messaging;

namespace ServiceBusManagementConsole
{
    public class ManagementHelper
    {
        private NamespaceManager m_NamespaceManager;

        public ManagementHelper(string connectionString)
        {
            m_NamespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            Console.WriteLine("Service bus address {0}", m_NamespaceManager.Address);
        }

        public void CreateQueue(string queuePath)
        {
            Console.WriteLine("Creating Queue {0}...", queuePath);
            var description = GetQueueDescription(queuePath);
            var queue = m_NamespaceManager.CreateQueue(description);
            Console.WriteLine("Done!");
        }

        public void ListQueues()
        {
            IEnumerable<QueueDescription> queueDescriptions = m_NamespaceManager.GetQueues();
            Console.WriteLine("Listing Queues...");
            foreach (var queueDescription in queueDescriptions)
            {
                Console.WriteLine("\t{0}", queueDescription.Path);
            }
            Console.WriteLine("Done");
        }

        public void GetQueue(string queuePath)
        {
            var queueDescription = m_NamespaceManager.GetQueue(queuePath);

            Console.WriteLine("Queue Path:    {0}", queueDescription.Path);
            Console.WriteLine("Message Count:    {0}", queueDescription.MessageCount);
            Console.WriteLine("Requires Session:    {0}", queueDescription.RequiresSession);
            Console.WriteLine("Requires Duplicate Detection:    {0}", queueDescription.RequiresDuplicateDetection);
            Console.WriteLine("Duplicate Detection History Time Window:    {0}", queueDescription.DuplicateDetectionHistoryTimeWindow);
            Console.WriteLine("Lock Duration:    {0}", queueDescription.LockDuration);
            Console.WriteLine("Default Message Time To Live:    {0}", queueDescription.DefaultMessageTimeToLive);
            Console.WriteLine("Enable Dead Lettering On Message Expiration:    {0}", queueDescription.EnableDeadLetteringOnMessageExpiration);
            Console.WriteLine("Enabled Batched Operations:    {0}", queueDescription.EnableBatchedOperations);
            Console.WriteLine("Message Size in Megabytes:    {0}", queueDescription.MaxSizeInMegabytes);
            Console.WriteLine("Max Delivery Count:    {0}", queueDescription.MaxDeliveryCount);
            Console.WriteLine("Is Readonly:    {0}", queueDescription.IsReadOnly);
        }

        public void DeleteQueue(string queuePath)
        {
            Console.WriteLine("Deleting Queue {0}...", queuePath);
            if (m_NamespaceManager.QueueExists(queuePath))
            {
                m_NamespaceManager.DeleteQueue(queuePath);
                Console.WriteLine("Done!");
            }
            else
            {
                Console.WriteLine("Queue doesn't exisit");
            }
        }

        private QueueDescription GetQueueDescription(string queuePath)
        {
            return new QueueDescription(queuePath)
            {
                RequiresDuplicateDetection = true,
                DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(10),
                RequiresSession = false,
                MaxDeliveryCount = 20,
                DefaultMessageTimeToLive = TimeSpan.FromHours(1),
                EnableDeadLetteringOnMessageExpiration = true
            };
        }

        public void ListTopicsAndSubscriptions()
        {
            IEnumerable<TopicDescription> topicDescriptions = m_NamespaceManager.GetTopics();
            Console.WriteLine("Listing Topics and Subscriptions...");
            foreach (var topicDescription in topicDescriptions)
            {
                Console.WriteLine("\t{0}", topicDescription.Path);
                IEnumerable<SubscriptionDescription> subscriptionDescriptions = m_NamespaceManager.GetSubscriptions(topicDescription.Path);
                foreach (var subscriptionDescription in subscriptionDescriptions)
                {
                    Console.WriteLine("\t\t{0}", subscriptionDescription.Name);
                }
            }
            Console.WriteLine("Done!");
        }

        public void CreateTopic(string topicPath)
        {
            Console.WriteLine("Creating Topic {0}...", topicPath);
            if (m_NamespaceManager.TopicExists(topicPath))
            {
                Console.WriteLine("Topic Already Exists!");
                return;
            }
            var topicDescription = new TopicDescription(topicPath)
            {

            };
            var description = m_NamespaceManager.CreateTopic(topicDescription);
            Console.WriteLine("Done!");
        }

        public void CreateSubscription(string topicPath, string subscriptionName)
        {
            Console.WriteLine("Creating Subscription {0}/subscriptions/{1}", topicPath, subscriptionName);
            if (!m_NamespaceManager.TopicExists(topicPath))
                CreateTopic(topicPath);
            if (m_NamespaceManager.SubscriptionExists(topicPath, subscriptionName))
            {
                Console.WriteLine("Subscription Already Exists!");
                return;
            }
            var subscriptionDescription = new SubscriptionDescription(topicPath, subscriptionName)
            {

            };
            var description = m_NamespaceManager.CreateSubscription(subscriptionDescription);
            Console.WriteLine("Done!");
        }
    }
}
