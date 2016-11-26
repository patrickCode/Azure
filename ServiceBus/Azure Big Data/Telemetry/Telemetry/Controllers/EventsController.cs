using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using NLog;
using Telemetry.Api.Analytics.Spec;
using Telemetry.Core;
using System.Configuration;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Telemetry.Api.Controllers
{
    public class EventsController : ApiController
    {
        private readonly Logger _log;
        private readonly IEventSender _sender;
        
        public EventsController(IEventSender sender)
        {
            _sender = sender;
            _log = this.GetLogger();
        }

        //The return will only contain the return message (based on if the event has been successfully processed)
        //The request will contain an array of events, but the schema is not fixed (typical BigData).
        //So we wont be working with Model (instead only with HttpResponse and HttpRequest Message.
        public async Task<HttpResponseMessage> Post(HttpRequestMessage requestMessage)
        {
            var json = string.Empty;
            var deviceId = Request.GetDeviceId(); //The device id is read from the header because it is same for all the events in the post request
            JArray events;
            try
            {
                json = await requestMessage.Content.ReadAsStringAsync();
                dynamic request = JObject.Parse(json); //dynamic JSON has been used becasue we are not fixing the JSON schema
                events = (JArray) request.events;

                _log.TraceEvent("ParseEvent",   //Disable Trace in higher environment
                    new Facet("deviceId", deviceId), 
                    new Facet("eventCount", events.Count.ToString()));
            }
            catch (Exception error)
            {
                _log.ErrorEvent("ParseEvent", error, new Facet("json", json));
                throw;
            }

            if (Boolean.Parse(ConfigurationManager.AppSettings["Telemetry.DeviceEvents.SendToEventhub"]))
            {
                try
                {
                    await _sender.SendEventsAsync(events, deviceId);    //Wait for the sending to be over, becasue based on the result we might have to change the response
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    var errorId = _log.ErrorEvent("SendEvents", ex);
                    var error = new {errorId = errorId};
                    var errorJson = JsonConvert.SerializeObject(error);

                    return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(errorJson)
                    };
                }
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

    }
}
