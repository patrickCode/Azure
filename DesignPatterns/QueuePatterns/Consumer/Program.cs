using System;
using Common.Table;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Consumer
{
    class Program
    {
        private static Sender _sender;
        private static TableProvider<ServiceRequestEntity> _serviceRequestTableProvider;
        private static StatusChecker _statusChecker;
        static void Main(string[] args)
        {
            var storageAccountName = ConfigurationManager.AppSettings["Microsoft.Storage.AccountName"];
            var storageAccountKey = ConfigurationManager.AppSettings["Microsoft.Storage.AccountKey"];
            _serviceRequestTableProvider = new TableProvider<ServiceRequestEntity>(storageAccountName, storageAccountKey, "ServiceRequest");
            var statusTableProvider = new TableProvider<StatusTableEntity>(storageAccountName, storageAccountKey, "Status");

            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var topicName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.TopicName"];
            _sender = new Sender(connectionString, topicName, statusTableProvider);

            _statusChecker = new StatusChecker(statusTableProvider);

            /*
            var tracker = SendDummyServiceRequest();
            var sku = _statusChecker.GetSku(tracker, 2);

            if (string.IsNullOrEmpty(sku))
                Console.WriteLine($"Timeout. Unable to get the SKU. Try again later");
            else if (sku.Equals("Failed"))
                Console.WriteLine($"Generation Failed");
            else
                Console.WriteLine($"SKU Generated {sku}");
                */
            SendMultipleRequests(10);
            Console.ReadLine();
        }

        public static string SendDummyServiceRequest()
        {
            var random = new Random();
            var id = random.Next(1, int.MaxValue);
            var tracker = Guid.NewGuid();
            var serviceRequest = new ServiceRequest()
            {
                Id = id,
                ClientName = "Console",
                Command = "Create Part Number",
                Tracker = tracker,
                Type = "PartNumber"
            };

            var entity = new ServiceRequestEntity(serviceRequest);
            _serviceRequestTableProvider.Add(entity);

            _sender.SendRequest(serviceRequest);
            Console.WriteLine($"Service Request Send. Id = {id}. Tracker - {tracker}");
            return tracker.ToString();
        }

        public static void SendMultipleRequests(int requestCount = 10)
        {
            var random = new Random();
            var trackers = new List<string>();
            var taskList = new List<Task>();

            for (var iterator = 1; iterator <= requestCount; iterator++)
            {
                var task = Task.Run(() =>
                {
                    var id = random.Next(1, int.MaxValue);
                    var tracker = Guid.NewGuid();
                    var serviceRequest = new ServiceRequest()
                    {
                        Id = id,
                        ClientName = "Console",
                        Command = "Create Part Number",
                        Tracker = tracker,
                        Type = "PartNumber"
                    };

                    var entity = new ServiceRequestEntity(serviceRequest);
                    _serviceRequestTableProvider.Add(entity);

                    _sender.SendRequest(serviceRequest);
                    Console.WriteLine($"Service Request Send. Id = {id}. Tracker - {tracker}");
                    trackers.Add(tracker.ToString());
                });
                taskList.Add(task);
            }

            Task.WaitAll(taskList.ToArray());

            Parallel.ForEach(trackers, (tracker) =>
            {
                var sku = _statusChecker.GetSku(tracker, 2);

                if (string.IsNullOrEmpty(sku))
                    Console.WriteLine($"Timeout. Unable to get the SKU. Try again later. - {tracker}");
                else if (sku.Equals("Failed"))
                    Console.WriteLine($"Generation Failed - {tracker}");
                else
                    Console.WriteLine($"SKU Generated {sku} - {tracker}");
            });

            Console.WriteLine("All Done");
        }
    }
}