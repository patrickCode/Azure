using System;
using Receiver;
using Common.Table;
using System.Threading;

namespace Service
{
    public class PartNumberProcessor : Processor
    {
        private readonly TableProvider<ServiceRequestEntity> _serviceRequestTableProvider;
        private readonly TableProvider<StatusTableEntity> _statusTableProvider;
        public PartNumberProcessor(MessageReceiver receiver, TableProvider<ServiceRequestEntity> serviceRequestTableProvider, TableProvider<StatusTableEntity> statusTableProvider) : base(receiver)
        {
            _serviceRequestTableProvider = serviceRequestTableProvider;
            _statusTableProvider = statusTableProvider;
        }

        public override void Process(object sender, MessageReceivedArgs argument)
        {
            var request = argument.Request;
            Console.WriteLine($"Part Number Request Processing - {request.Tracker}");
            try
            {
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
                var errorNumber = random.Next(0, 3);
                if (errorNumber == 2)
                    throw new Exception($"Failed to create Part Number for {request.Tracker}");

                request.Sku = "PNR-" + random.Next(10000, 99999);
                _serviceRequestTableProvider.Update(new ServiceRequestEntity(request));

                status.Status = "Completed";
                status.ProcessedTime = DateTime.UtcNow;
                status.Result = request.Sku;

                _statusTableProvider.Update(status);
            }
            catch (Exception exception)
            {
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
                status.Status = "Failed";
                _statusTableProvider.Update(status);
                Console.WriteLine($"Part Number Request Processing has failed- {request.Tracker}");
                throw;
            }
        }
    }
}