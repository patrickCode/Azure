using System;
using Receiver;
using Common.Table;
using System.Configuration;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var topicName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.TopicName"];
            var costRateReceiver = new MessageReceiver(connectionString, topicName, "costrate-sub", "CostRate", 2);
            var partNumberReceiver = new MessageReceiver(connectionString, topicName, "pnr-sub", "PartNumber", 2);

            var storageAccountName = ConfigurationManager.AppSettings["Microsoft.Storage.AccountName"];
            var storageAccountKey = ConfigurationManager.AppSettings["Microsoft.Storage.AccountKey"];
            var serviceRequestTableProvider = new TableProvider<ServiceRequestEntity>(storageAccountName, storageAccountKey, "ServiceRequest");
            var statusTableProvider = new TableProvider<StatusTableEntity>(storageAccountName, storageAccountKey, "Status");

            var costRateProcessor = new CostRateProcessor(costRateReceiver, serviceRequestTableProvider, statusTableProvider);
            var partNumberProcessor = new PartNumberProcessor(partNumberReceiver, serviceRequestTableProvider, statusTableProvider);

            costRateProcessor.Start();
            partNumberProcessor.Start();

            Console.ReadLine();
        }
    }
}