using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Table
{
    public class StatusInfo
    {
        public StatusInfo() { }
        public Guid TrackingGuid { get; set; }
        public string Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ProcessedTime { get; set; }
        public string Result { get; set; }
        public string Remarks { get; set; }
        public string Client { get; set; }
    }
}
