using System;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus;

namespace Json.Sender
{
    class Program
    {
        private static string _connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        private static string _queueName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.Queue.Name"];
        private static string _dupQueueName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.DupQueue.Name"];
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to send message");
            Console.ReadLine();

            //var sr = new ServiceRequest()
            //{
            //    Id = 2,
            //    CustomerId = 67,
            //    Description = "JSON Message Testing",
            //    Type = "Plumbing"
            //};

            //SendJsonMessage(sr);

            SendDuplicateServiceRequests();
            Console.ReadLine();
        }

        private static void SendJsonMessage(Object content)
        {
            var jsonStr = JsonConvert.SerializeObject(content);

            Console.WriteLine("Message > " + jsonStr);

            var message = new BrokeredMessage(jsonStr);

            message.ContentType = "application/json";
            message.Label = content.GetType().ToString();
            Console.WriteLine("Type > " + message.Label);

            Console.WriteLine("Sending Message ...");

            var queueClient = QueueClient.CreateFromConnectionString(_connectionString, _queueName);

            queueClient.Send(message);

            Console.WriteLine("Done!");

            queueClient.Close();
        }

        private static void SendDuplicateServiceRequests()
        {
            var namespaceMgr = NamespaceManager.CreateFromConnectionString(_connectionString);
            if (namespaceMgr.QueueExists(_dupQueueName))
                namespaceMgr.DeleteQueue(_dupQueueName);
            var queueDescription = new QueueDescription(_dupQueueName)
            {
                RequiresDuplicateDetection = true,
                DuplicateDetectionHistoryTimeWindow = TimeSpan.FromHours(1)
            };
            namespaceMgr.CreateQueue(queueDescription);

            var queueClient = QueueClient.CreateFromConnectionString(_connectionString, _dupQueueName);

            for (var iterator = 1; iterator < 20; iterator++)
            {
                var srId = iterator;
                if (iterator % 5 == 0)
                    srId = iterator - 1;

                var sr = new ServiceRequest()
                {
                    Id = srId,
                    CustomerId = srId * 98,
                    Description = "Test Description",
                    Type = "Other"
                };

                var message = new BrokeredMessage(sr)
                {
                    MessageId = srId.ToString(),
                    ContentType = "application/json",
                    Label = typeof(ServiceRequest).ToString()
                };

                Console.WriteLine("Sending Message ...\n" + sr);
                queueClient.Send(message);
                Console.WriteLine("Done");
            }
        }
    }
}