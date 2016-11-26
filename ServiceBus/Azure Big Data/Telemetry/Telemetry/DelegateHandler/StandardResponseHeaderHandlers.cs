using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Telemetry.Api.DelegateHandler
{
    public class StandardResponseHeaderHandlers : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage,
            CancellationToken cancellationToken)
        {
            return await base.SendAsync(requestMessage, cancellationToken).ContinueWith(task =>
            {
                task.Result.AddStandardHeader();
                return task.Result;
            }, cancellationToken);
        }
    }
}