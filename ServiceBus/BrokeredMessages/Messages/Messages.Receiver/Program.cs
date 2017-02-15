using System;
using System.Threading;
using Messages.Contracts;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace Messages.Receiver
{
    class Program
    {
        private static string _connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        private static string _queueName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.Queue.Name"];
        private static QueueClient _queueClient;
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to receive Service Requests");
            Console.ReadLine();
            _queueClient = QueueClient.CreateFromConnectionString(_connectionString, _queueName);

            //ReceiveMessage();

            ReceiveMessageUsingOnMessage(1);
            //ReceiveMessageUsingOnMessage(10);
            //ReceiveMessageUsingOnMessage(30);

            StopReceiving();
        }

        private static void ReceiveMessage()
        {   
            while (true)
            {
                Console.WriteLine("Receiving ...");
                var message = _queueClient.Receive(TimeSpan.FromSeconds(10));

                if (message != null)
                {
                    try
                    {
                        Console.WriteLine("Received > " + message.Label);
                        if (!message.Label.Equals("Service Request"))
                        {
                            Console.WriteLine("Not a Service Request");
                            message.Complete();
                            continue;
                        }
                        var serviceRequest = message.GetBody<ServiceRequest>();
                        Console.WriteLine("Processing Message");
                        ProcessServiceRequest(serviceRequest);
                        Console.WriteLine("Marking Message as complete");
                        message.Complete();
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(string.Format("Some error {0} ocurred. Abandoning message.", error.Message));
                        message.Abandon(); //Dead-lettering
                    }
                }
            }
        }

        private static void ReceiveMessageUsingOnMessage(int concurrentMessages)
        {
            _queueClient = QueueClient.CreateFromConnectionString(_connectionString, _queueName);

            var options = new OnMessageOptions()
            {
                AutoComplete = false,
                MaxConcurrentCalls = concurrentMessages,
                AutoRenewTimeout = TimeSpan.FromSeconds(30)
            };

            //Creates a message pump
            _queueClient.OnMessage(message =>
            {
                var sr = message.GetBody<ServiceRequest>();
                ProcessServiceRequest(sr);
                message.Complete();
            }, options);

            Console.WriteLine("Type exit to stop receiving messages");
            var resposne = Console.ReadLine();
            if (resposne.Equals("exit"))
                StopReceiving();
        }

        private static void ProcessServiceRequest(ServiceRequest sr)
        {
            Thread.Sleep(3000);
            Console.WriteLine("ID > " + sr.Id);
            Console.WriteLine("Type > " + sr.Type);
            Console.WriteLine("Description > " + sr.Description);
            Console.WriteLine("Customer ID > " + sr.CustomerId);
        }

        private static void StopReceiving()
        {
            _queueClient.Close();
        }
    }
}
