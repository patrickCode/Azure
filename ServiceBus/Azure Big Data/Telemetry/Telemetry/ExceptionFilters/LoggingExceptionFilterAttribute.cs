using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using NLog;
using Telemetry.Core;

namespace Telemetry.Api.ExceptionFilters
{
    public class LoggingExceptionFilterAttribute: ExceptionFilterAttribute
    {
        private readonly Logger _log;

        public LoggingExceptionFilterAttribute()
        {
            _log = this.GetLogger();
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            object requestId;
            actionExecutedContext.Request.Properties.TryGetValue("requestId", out requestId);
            if (requestId == null) requestId = Guid.NewGuid();
            var errorId = _log.ErrorEvent("OnException", actionExecutedContext.Exception,
                new Facet {Name = "RequestId", Value = requestId.ToString()});

            var error = new {errorId = errorId};
            var content = JsonConvert.SerializeObject(error);

            actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                ReasonPhrase = "Server Error.",
                Content = new StringContent(content),
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }
}