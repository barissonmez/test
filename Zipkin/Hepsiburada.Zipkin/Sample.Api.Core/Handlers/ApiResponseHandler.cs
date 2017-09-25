using System.Net.Http;
using System.Text;
using System.Web.Http;
using Sample.Api.Core.Models;

namespace Sample.Api.Core.Handlers
{
    public class ApiResponseHandler : DelegatingHandler
    {
        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            return BuildApiResponse(request, response);
        }

        private static HttpResponseMessage BuildApiResponse(HttpRequestMessage request, HttpResponseMessage response)
        {
            object content = null;
            string errorMessage = string.Empty;

            ValidateResponse(response, ref content, ref errorMessage);

            // Yeni response'u custom olarak oluşturmuş olduğumuz wrapper sınıf ile baştan oluşturuyoruz.
            var newResponse = CreateHttpResponseMessage(request, response, content, errorMessage);

            // Header key'lerini baştan set et.
            foreach (var loopHeader in response.Headers)
            {
                newResponse.Headers.Add(loopHeader.Key, loopHeader.Value);
            }

            return newResponse;
        }

        private static HttpResponseMessage CreateHttpResponseMessage<T>(HttpRequestMessage request, HttpResponseMessage response, T content, string errorMessage)
        {
            return request.CreateResponse(response.StatusCode, new ApiResponse<T>(response.StatusCode, content, errorMessage));
        }

        private static void ValidateResponse(HttpResponseMessage response, ref object content, ref string errorMessage)
        {
            if (response.TryGetContentValue(out content) && !response.IsSuccessStatusCode)
            {
                HttpError error = content as HttpError;

                if (error != null)
                {
                    content = null;
                    StringBuilder sb = new StringBuilder();

                    foreach (var loopError in error)
                    {
                        sb.Append(string.Format("{0}: {1} ", loopError.Key, loopError.Value));
                    }

                    errorMessage = sb.ToString();
                }
            }
        }
    }
}
