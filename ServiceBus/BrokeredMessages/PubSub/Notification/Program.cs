using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification
{
    class Program
    {
        private static string _connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        private static string _topicName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.Topic.Name"];
        static void Main(string[] args)
        {
            Console.WriteLine("Subscribing to receive messages");
            var pushNotification = new PushNotification();
            Console.WriteLine("Subscribed to receive messages");

            var topicClient = TopicClient.CreateFromConnectionString(_connectionString, _topicName);

            while (true)
            {
                Console.Write("> ");
                var txtMessage = Console.ReadLine();
                if (txtMessage.Equals("exit"))
                    break;
                var message = new BrokeredMessage(txtMessage)
                {
                    ContentType = "text"
                };
                topicClient.Send(message);
            }
            topicClient.Close();
            Console.WriteLine("Notification Service completed ...\nPress any key to continue");
            Console.ReadLine();
        }

        static void SendNotification(string text)
        {

        }
    }
}
