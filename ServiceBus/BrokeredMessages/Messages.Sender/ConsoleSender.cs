using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Messages.Contracts;
using System.Threading;

namespace Messages.Sender
{
    public class ConsoleSender
    {
        private static string _connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        private static string _queueName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.Queue.Name"];
        private const string _textMsg = "The quick brown fox jumped over the fence";
        static void Main(string[] args)
        {
            //Console.WriteLine("Sending Text Message ...");
            //SendTextMessage(_textMsg);

            //SendControlMessage();

            var serviceRequest = new ServiceRequest()
            {
                Id = 1,
                CustomerId = 432,
                Description = "Test Service Request",
                Type = "Electrical"
            };
            SendServiceRequest(serviceRequest);

            //SendMessagesInLoop();
            SendRapidMessagesInLoop();
            Console.ReadLine();
        }

        private static void SendTextMessage(string text, bool sendSync = true)
        {
            var queueClient = QueueClient.CreateFromConnectionString(_connectionString, _queueName);

            Console.WriteLine("Sending Message ...");

            foreach (var letter in text)
            {
                var message = new BrokeredMessage();
                message.Label = letter.ToString();

                if (sendSync)
                {
                    queueClient.Send(message);
                    Console.WriteLine("Message Sent > " + message.Label);
                }
                else
                {
                    var sendTask = new Task(() =>
                    {
                        queueClient.Send(message);
                        Console.WriteLine("Message Sent > " + message.Label);
                    });

                    sendTask.Start();
                }
            }
            queueClient.Close();
        }

        private static void SendControlMessage()
        {
            var queueClient = QueueClient.CreateFromConnectionString(_connectionString, _queueName);

            var message = new BrokeredMessage()
            {
                Label = "Control"
            };
            message.Properties.Add("DeviceId", 386);
            message.Properties.Add("Event", "Service Request Added");
            message.Properties.Add("Time", DateTime.UtcNow);

            var sendAgain = true;

            while (sendAgain)
            {
                Console.WriteLine("Sending Control Message ...");
                //Message cannot be send for the 2nd time
                message = message.Clone();
                queueClient.Send(message);
                Console.WriteLine("Done!");

                Console.WriteLine("Send Again?");
                var resposne = Console.ReadLine();
                if (resposne.ToString() == "N" || resposne.ToString() == "n")
                {
                    sendAgain = false;
                }
            }

            queueClient.Close();
        }

        private static void SendServiceRequest(ServiceRequest sr)
        {
            var queueClient = QueueClient.CreateFromConnectionString(_connectionString, _queueName);


            var message = new BrokeredMessage(sr)
            {
                Label = "Service Request"
            };
            message.Properties.Add("Time", DateTime.UtcNow);

            Console.WriteLine("Message Size > " + message.Size);
            Console.WriteLine("Sending Data Message ...");
            queueClient.Send(message);
            Console.WriteLine("Done!");

            queueClient.Close();
            Console.WriteLine("Message Size > " + message.Size);
        }

        private static void SendMessagesInLoop()
        {
            for (int i = 0; i < 10; i++)
            {
                var serviceRequest = new ServiceRequest()
                {
                    Id = i,
                    CustomerId = 432 + i * 7,
                    Description = "Test Service Request",
                    Type = "Electrical"
                };
                SendServiceRequest(serviceRequest);
                Thread.Sleep(5000);
            }
        }

        private static void SendRapidMessagesInLoop()
        {
            for (int i = 1; i <= 50; i++)
            {
                var serviceRequest = new ServiceRequest()
                {
                    Id = i,
                    CustomerId = 432 + i * 7,
                    Description = "Test Service Request",
                    Type = "Electrical"
                };
                SendServiceRequest(serviceRequest);
                if (i % 10 == 0)
                    Thread.Sleep(5000);
            }
        }
    }
}
