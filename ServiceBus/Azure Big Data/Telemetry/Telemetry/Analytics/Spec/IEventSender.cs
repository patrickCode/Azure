using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Telemetry.Api.Analytics.Spec
{
    public interface IEventSender
    {
        Task SendEventsAsync(JArray events, string deviceId);
    }
}
