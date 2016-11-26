using System.Net.Http.Headers;
using Telemetry;

namespace System.Net.Http
{
    public static class HttpResponseMessageExtensions
    {
        private static MediaTypeHeaderValue _mediaType;
        private static string _versionNumber;
        private static string _buildNumber;

        static HttpResponseMessageExtensions()
        {
            _mediaType = new MediaTypeHeaderValue("application/json");
            var version = typeof (WebApiApplication).Assembly.GetName().Version;
            _buildNumber = version.ToString();
            _versionNumber = string.Format("{0}.{1}", version.Major, version.Minor);
        }

        public static void AddStandardHeader(this HttpResponseMessage responseMessage)
        {
            AddResponseHeaders(responseMessage.Headers);
            if (responseMessage.Content != null)
            {
                AddContentHeaders(responseMessage.Content.Headers);
            }
        }

        private static void AddResponseHeaders(HttpResponseHeaders headers)
        {
            headers.Add("x-api-version", _versionNumber);
            headers.Add("x-api-build", _buildNumber);
        }

        private static void AddContentHeaders(HttpContentHeaders headers)
        {
            headers.ContentType = _mediaType;
        } 
    }
}