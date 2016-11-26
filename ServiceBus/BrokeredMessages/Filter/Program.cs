using System;
using Microsoft.ServiceBus;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace Filter
{
    class Program
    {
        private static string _connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        private static string _topicName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.Topic.Name"];

        private static string _defaultSubscription = "DefaultSubscription";
        private static string _hydSubscription = "HydSubscription";
        private static string _kolSubscription = "KolSubscription";
        private static string _hydAndExpSubscription = "HydExpSubscription";
        private static string _indiaSubscription = "IndSubscription";

        private static TopicClient _topicClient;
        static void Main(string[] args)
        {
            _topicClient = TopicClient.CreateFromConnectionString(_connectionString, _topicName);

            Console.WriteLine("Creating Topics and Subscriptions ...");
            CreateSubscriptionsWithFilters();
            Console.WriteLine("Topics and Subscriptions Created");

            Console.WriteLine("Press enter to send Service Requests");
            Console.ReadLine();
            SendServiceRequests();

            Console.WriteLine("Press enter to receive messages");
            Console.ReadLine();
            ReceiveMessages();

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        static void CreateSubscriptionsWithFilters()
        {
            var namespaceMgr = NamespaceManager.CreateFromConnectionString(_connectionString);

            if (!namespaceMgr.TopicExists(_topicName))
                namespaceMgr.CreateTopic(_topicName);

            //Default Subscription
            if (!namespaceMgr.SubscriptionExists(_topicName, _defaultSubscription))
                namespaceMgr.CreateSubscription(_topicName, _defaultSubscription);

            //Hyderabad Subscription
            if (!namespaceMgr.SubscriptionExists(_topicName, _hydSubscription))
                namespaceMgr.CreateSubscription(_topicName, _hydSubscription, new SqlFilter("City = 'Hyderabad'"));

            //Kolkata Subscription
            if (!namespaceMgr.SubscriptionExists(_topicName, _kolSubscription))
                namespaceMgr.CreateSubscription(_topicName, _kolSubscription, new SqlFilter("City = 'Kolkata'"));

            //Hyderabad & Expensive Subscription
            if (!namespaceMgr.SubscriptionExists(_topicName, _hydAndExpSubscription))
                namespaceMgr.CreateSubscription(_topicName, _hydAndExpSubscription, new SqlFilter("City = 'Hyderabad' AND Amount > 100000"));

            //India Subscription
            if (!namespaceMgr.SubscriptionExists(_topicName, _indiaSubscription))
                namespaceMgr.CreateSubscription(_topicName, _indiaSubscription, new CorrelationFilter("IND"));
        }

        static void ReceiveMessages()
        {
            var namespaceMgr = NamespaceManager.CreateFromConnectionString(_connectionString);
            foreach (var subDescription in namespaceMgr.GetSubscriptions(_topicName))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("   Receiving from subscription {0}...", subDescription.Name);

                var client = SubscriptionClient.CreateFromConnectionString(_connectionString, _topicName, subDescription.Name);

                Console.ForegroundColor = ConsoleColor.Green;
                while (true)
                {
                    var msg = client.Receive(TimeSpan.FromSeconds(2));
                    if(msg != null)
                    {
                        if (msg.ContentType.Equals("ServiceRequest"))
                        {
                            var sr = msg.GetBody<ServiceRequest>();
                            msg.Complete();
                            Console.WriteLine(sr);
                        } else
                        {
                            Console.WriteLine("Unknown Message Received");
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                        break;
                    }
                }
                client.Close();
            }
            Console.ResetColor();
        }

        static void SendServiceRequest(ServiceRequest sr)
        {
            var message = new BrokeredMessage(sr);
            message.ContentType = "ServiceRequest";

            //Promoting Properties
            message.Properties.Add("City", sr.City);
            message.Properties.Add("Amount", sr.Amount);
            message.CorrelationId = sr.Country;

            Console.WriteLine("Sending Message ...");
            _topicClient.Send(message);
            Console.WriteLine("Done");
        }
        static void SendServiceRequests()
        {
            var sr1 = new ServiceRequest()
            {
                Id = 1,
                Amount = 12000,
                City = "Kolkata",
                Country = "IND"
            };
            var sr2 = new ServiceRequest()
            {
                Id = 2,
                Amount = 12000000,
                City = "Hyderabad",
                Country = "IND"
            };
            var sr3 = new ServiceRequest()
            {
                Id = 3,
                Amount = 31000,
                City = "California",
                Country = "USA"
            };
            var sr4 = new ServiceRequest()
            {
                Id = 5,
                Amount = 7200000,
                City = "Hyderabad",
                Country = "IND"
            };
            var sr5 = new ServiceRequest()
            {
                Id = 6,
                Amount = 100000,
                City = "Shanghai",
                Country = "CHN"
            };
            var sr6 = new ServiceRequest()
            {
                Id = 7,
                Amount = 79000,
                City = "Kolkata",
                Country = "IND"
            };
            var sr7 = new ServiceRequest()
            {
                Id = 8,
                Amount = 8000000,
                City = "Delhi",
                Country = "IND"
            };
            var sr8 = new ServiceRequest()
            {
                Id = 8,
                Amount = 12300,
                City = "London",
                Country = "UK"
            };
            var sr9 = new ServiceRequest()
            {
                Id = 9,
                Amount = 12000,
                City = "Dubai",
                Country = "UAE"
            };
            var sr10 = new ServiceRequest()
            {
                Id = 10,
                Amount = 7000,
                City = "Hyderabad",
                Country = "IND"
            };

            SendServiceRequest(sr1);
            SendServiceRequest(sr2);
            SendServiceRequest(sr3);
            SendServiceRequest(sr4);
            SendServiceRequest(sr5);
            SendServiceRequest(sr6);
            SendServiceRequest(sr7);
            SendServiceRequest(sr8);
            SendServiceRequest(sr9);
            SendServiceRequest(sr10);
        }
    }
}
