using System;

namespace Common.Table
{
    public class ServiceRequest
    {
        public ServiceRequest() { }
        public int Id { get; set; }
        public Guid Tracker { get; set; }
        public string Command { get; set; }
        public string Type { get; set; }
        public string ClientName { get; set; }
        public string Sku { get; set; }
    }
}