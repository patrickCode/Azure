using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Configuration;
using Microsoft.ServiceBus;

namespace Publisher
{
    class Program
    {
        static string EventHubName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.EventHub"];
        static string ServiceBusConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        static EventHubClient _eventHubClient;
        static void Main(string[] args)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ServiceBusConnectionString);
            var hubs = namespaceManager.GetEventHubs();


            var ehd = new EventHubDescription(EventHubName);


            _eventHubClient = EventHubClient.CreateFromConnectionString(ServiceBusConnectionString, EventHubName);
            //MessageSender();
        }

        static void MessageSender()
        {
            while (true)
            {
                try
                {
                    Console.Write("> ");
                    var message = Console.ReadLine();
                    var eventData = new EventData(Encoding.UTF8.GetBytes(message));
                    _eventHubClient.Send(eventData);
                }
                catch (Exception error)
                {
                    Console.WriteLine("Unable to Send Message -> {0}", error.Message);
                }

                Thread.Sleep(100);
            }
        }
    }
}
;