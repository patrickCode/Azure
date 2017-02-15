using System;
using Receiver;
using Common.Table;
using System.Threading;

namespace Service
{
    public class CostRateProcessor : Processor
    {
        private readonly TableProvider<ServiceRequestEntity> _serviceRequestTableProvider;
        private readonly TableProvider<StatusTableEntity> _statusTableProvider;
        public CostRateProcessor(MessageReceiver receiver, TableProvider<ServiceRequestEntity> serviceRequestTableProvider, TableProvider<StatusTableEntity> statusTableProvider) : base(receiver)
        {
            _serviceRequestTableProvider = serviceRequestTableProvider;
            _statusTableProvider = statusTableProvider;
        }

        public override void Process(object sender, MessageReceivedArgs argument)
        {
            var request = argument.Request;
            var status = _statusTableProvider.Get("ServiceRequestCommand", request.Tracker.ToString());
            if (status == null)
            {
                var statusInfo = new StatusInfo()
                {
                    Client = request.ClientName,
                    CreatedTime = DateTime.UtcNow,
                    Status = "Created",
                    TrackingGuid = request.Tracker
                };
                status = new StatusTableEntity(statusInfo);
            }
            status.Status = "Processing";
            _statusTableProvider.Update(status);

            Thread.Sleep(3000);
            var random = new Random();
            request.Sku = "CSR-" + random.Next(10000, 99999);
            _serviceRequestTableProvider.Update(new ServiceRequestEntity(request));

            status.Status = "Completed";
            status.ProcessedTime = DateTime.UtcNow;
            status.Result = request.Sku;

            _statusTableProvider.Update(status);
            Console.WriteLine($"Cost Rate Request Processing Complete - {request.Tracker}");
        }
    }
}