using System;
using Common.Table;

namespace Receiver
{
    public class MessageReceivedArgs
    {
        public ServiceRequest Request { get; set; }
        public DateTime MessageCreatedAt { get; set; }
    }
}