using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.Net.Http
{
    public static class HttpRequestMessageExtensions
    {
        public static string GetDeviceId(this HttpRequestMessage request)
        {
            return request.GetHeaderValue("x-device-id");
        }

        private static string GetHeaderValue(this HttpRequestMessage request, string headerName)
        {
            IEnumerable<string> headerValues;
            request.Headers.TryGetValues(headerName, out headerValues);
            var headerValuesArr = headerValues as string[] ?? headerValues.ToArray();
            return (headerValues != null && headerValuesArr.FirstOrDefault() != null)
                ? headerValuesArr.FirstOrDefault().Trim()
                : string.Empty;
        }

        public static bool IsContentZip(this HttpRequestMessage requestMessage)
        {
            return requestMessage.Content.Headers.ContentEncoding.Contains("gzip") ||
                   (requestMessage.Content.Headers.ContentType != null &&
                    requestMessage.Content.Headers.ContentType.MediaType == "application/gzip");
        }
    }
}