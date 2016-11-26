using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Telemetry.Core;

namespace NLog
{
    public static class LoggerExtensions
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
        public static void TraceEvent(this Logger log, string name, params Facet[] facets)
        {
            LogEvent(log, l => l.IsTraceEnabled, (l, e) => l.Trace(e), name, facets);
        }

        public static void DebugEvent(this Logger log, string name, params Facet[] facets)
        {
            LogEvent(log, l => l.IsDebugEnabled, (l, e) => l.Debug(e), name, facets);
        }
        public static void InfoEvent(this Logger log, string name, params Facet[] facets)
        {
            LogEvent(log, l => l.IsInfoEnabled, (l, e) => l.Info(e), name, facets);
        }

        public static Guid ErrorEvent(this Logger log, string name, Exception error, params Facet[] facets)
        {
            var facetList = new List<Facet>(facets)
            {
                new Facet {Name = "_exception", Value = error.ToString()}
            };
            var errorId = Guid.NewGuid();
            facetList.Add(new Facet {Name = "_errorId", Value = errorId.ToString()});
            LogEvent(log, l => l.IsErrorEnabled, (l, e) => l.Error(e), name, facetList.ToArray());
            return errorId;
        }

        private static void LogEvent(Logger log, Func<Logger, bool> isEnabled, Action<Logger, string> logAction,
            string name, params Facet[] facets)
        {
            var loggerEnabled = Boolean.Parse(System.Configuration.ConfigurationManager.AppSettings["LoggerEnabled"]);
            if (!loggerEnabled || !isEnabled(log)) return;
            var facetList = new List<Facet>(facets)
            {
                new Facet {Name = "_loggerId", Value = log.GetHashCode().ToString()}
            };
            var @event = new Event(name, facetList);
            var json = JsonConvert.SerializeObject(@event, JsonSettings);
            logAction(log, json);
        }
    }
}
