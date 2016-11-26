using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Telemetry.Api.DelegateHandler
{
    //The code needs to preserve the whole pipeline so that this Handler can wrap another handler and pass control onto it.
    //This logic needs to run before the controller logic.
    public class GZipToJsonHandler: DelegatingHandler
    {
        public GZipToJsonHandler(HttpConfiguration config)
        {
            //Here we are retaining the standard handler pipeline, so that this can run after the custom handler has run
            InnerHandler = new HttpControllerDispatcher(config);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //If the content is not compressed then no pass the base handler
            if (!request.IsContentZip())
            {
                return await base.SendAsync(request, cancellationToken);    
            }

            var output = new MemoryStream();
            await request.Content.ReadAsStreamAsync().ContinueWith(t =>
            {
                var input = t.Result;
                //var input = File.ReadAllBytes(@"C:\Users\pratikb\Downloads\real-world-big-data-microsoft-azure\2-real-world-big-data-microsoft-azure-m2-exercise-files\after\Telemetry.Api.Tests.Acceptance\Resources\Requests\device-events-large.json.gz");
                using (var gzipStream = new GZipStream(input, CompressionMode.Decompress))
                {
                    gzipStream.CopyTo(output);
                    output.Flush();
                }
                output.Position = 0;
            }, cancellationToken);

            request.Content = new StreamContent(output);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}