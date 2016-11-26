using System;
using Json.Sender;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace Json.Receiver
{
    class Program
    {
        private static string _connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        private static string _queueName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.Queue.Name"];
        private static string _dupQueueName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.DupQueue.Name"];
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to start receiving messages");
            Console.ReadLine();

            //ProcessServiceRequest();
            ProcessDuplicateServiceRequests();
        }

        static void ProcessServiceRequest()
        {
            var queueClient = QueueClient.CreateFromConnectionString(_connectionString, _queueName);

            while(true)
            {
                var serviceRequest = queueClient.Receive();

                if (serviceRequest != null)
                {
                    Console.WriteLine("Message Received");

                    if (serviceRequest.ContentType.Equals("application/json"))
                    {
                        var content = serviceRequest.GetBody<string>();
                        Console.WriteLine("Full Content > " + content);
                        Console.WriteLine("Label > " + serviceRequest.Label);
                        Console.WriteLine();

                        if (serviceRequest.Label.Equals(typeof(ServiceRequest).ToString()))
                        {
                            var receivedServiceRequest = JsonConvert.DeserializeObject<ServiceRequest>(content);
                            Console.WriteLine("Service Request > " + receivedServiceRequest);
                        }
                    }
                }
                serviceRequest.Complete();
            }
        }

        static void ProcessDuplicateServiceRequests()
        {
            var queueClient = QueueClient.CreateFromConnectionString(_connectionString, _dupQueueName);

            while (true)
            {
                var serviceRequest = queueClient.Receive();

                if (serviceRequest != null)
                {
                    Console.WriteLine("Message Received");

                    if (serviceRequest.ContentType.Equals("application/json"))
                    {   
                        Console.WriteLine("Label > " + serviceRequest.Label);
                        Console.WriteLine();

                        if (serviceRequest.Label.Equals(typeof(ServiceRequest).ToString()))
                        {
                            var receivedServiceRequest = serviceRequest.GetBody<ServiceRequest>();
                            Console.WriteLine("Service Request > " + receivedServiceRequest);
                        }
                    }
                }
                serviceRequest.Complete();
            }
        }
    }
}
