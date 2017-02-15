using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Common.Table
{
    public class StatusTableEntity: TableEntity
    {
        public StatusTableEntity() { }
        public StatusTableEntity(StatusInfo statusInfo)
        {
            PartitionKey = "ServiceRequestCommand";
            RowKey = statusInfo.TrackingGuid.ToString();
            Status = statusInfo.Status;
            CreatedTime = statusInfo.CreatedTime;
            Result = string.IsNullOrEmpty(statusInfo.Result) ? "No Result Yet" : statusInfo.Result;
            Remarks = statusInfo.Remarks;
            ProcessedTime = statusInfo.ProcessedTime == default(DateTime) ? new DateTime(1601, 1, 1) : statusInfo.ProcessedTime;
            ClientName = statusInfo.Client;
        }
        public string Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ProcessedTime { get; set; }
        public string Result { get; set; }
        public string Remarks { get; set; }
        public string ClientName { get; set; }
    }
}