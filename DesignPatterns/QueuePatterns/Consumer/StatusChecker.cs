using System;
using Common.Table;
using System.Threading;

namespace Consumer
{
    public class StatusChecker
    {
        private readonly TableProvider<StatusTableEntity> _statusTableProvider;
        public StatusChecker(TableProvider<StatusTableEntity> statusTableProvider)
        {
            _statusTableProvider = statusTableProvider;
        }

        public StatusInfo GetStatus(string requestTracker)
        {
            var statusEntity = _statusTableProvider.Get("ServiceRequestCommand", requestTracker);
            if (statusEntity == null)
                return null;
            return Convert(statusEntity);
        }

        public string GetSku(string trackingGuid, int timeout = 60)
        {
            var timeSpent = TimeSpan.FromMinutes(0);
            var totalAllotedTime = TimeSpan.FromMinutes(2);
            var sku = string.Empty;
            do
            {
                var currentStatus = GetStatus(trackingGuid);
                if (currentStatus == null)
                    continue;
                if (currentStatus.Status.Equals("Completed"))
                    sku = currentStatus.Result;
                else if (currentStatus.Status.Equals("Failed"))
                    sku = "Failed";
                else
                    Thread.Sleep(400);
                timeSpent = timeSpent.Add(TimeSpan.FromMilliseconds(400));
            } while (string.IsNullOrEmpty(sku) && timeSpent < totalAllotedTime);
            return sku;
        }

        private StatusInfo Convert(StatusTableEntity statusTableEntity)
        {
            return new StatusInfo()
            {
                TrackingGuid = Guid.Parse(statusTableEntity.RowKey),
                Status = statusTableEntity.Status,
                CreatedTime = statusTableEntity.CreatedTime,
                Result = statusTableEntity.Result.Equals("No Result Yet") ? string.Empty : statusTableEntity.Result,
                Remarks = statusTableEntity.Remarks,
                ProcessedTime = statusTableEntity.ProcessedTime == default(DateTime) ? new DateTime(1601, 1, 1) : statusTableEntity.ProcessedTime,
                Client = statusTableEntity.ClientName
            };
        }
    }
}