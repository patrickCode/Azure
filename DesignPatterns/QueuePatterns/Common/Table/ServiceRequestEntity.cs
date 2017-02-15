using Microsoft.WindowsAzure.Storage.Table;

namespace Common.Table
{
    public class ServiceRequestEntity: TableEntity
    {
        public ServiceRequestEntity() { }
        public ServiceRequestEntity(ServiceRequest serviceRequest)
        {
            PartitionKey = serviceRequest.Type;
            RowKey = serviceRequest.Tracker.ToString();
            Id = serviceRequest.Id;
            Command = serviceRequest.Command;
            ClientName = serviceRequest.ClientName;
            Sku = string.IsNullOrEmpty(serviceRequest.Sku) ? "No SKU": serviceRequest.Sku;
        }
        public int Id { get; set; }
        public string Command { get; set; }
        public string ClientName { get; set; }
        public string Sku { get; set; }
    }
}