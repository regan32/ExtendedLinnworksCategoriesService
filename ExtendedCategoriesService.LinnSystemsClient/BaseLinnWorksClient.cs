// // <copyright file="BaseLinnWorksClient.cs" company="Paragon Software Group">
// // EXCEPT WHERE OTHERWISE STATED, THE INFORMATION AND SOURCE CODE CONTAINED
// // HEREIN AND IN RELATED FILES IS THE EXCLUSIVE PROPERTY OF PARAGON SOFTWARE
// // GROUP COMPANY AND MAY NOT BE EXAMINED, DISTRIBUTED, DISCLOSED, OR REPRODUCED
// // IN WHOLE OR IN PART WITHOUT EXPLICIT WRITTEN AUTHORIZATION FROM THE COMPANY.
// //
// // Copyright (c) 1994-2017 Paragon Software Group, All rights reserved.
// //
// // UNLESS OTHERWISE AGREED IN A WRITING SIGNED BY THE PARTIES, THIS SOFTWARE IS
// // PROVIDED "AS-IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// // LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// // PARTICULAR PURPOSE, ALL OF WHICH ARE HEREBY DISCLAIMED. IN NO EVENT SHALL THE
// // AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// // CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// // SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// // INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// // CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// // ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF NOT ADVISED OF
// // THE POSSIBILITY OF SUCH DAMAGE.
// // </copyright>

namespace ExtendedCategoriesService.LinnSystemsClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using EnsureThat;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions.Exceptions;
    using Newtonsoft.Json;

    internal class BaseLinnWorksClient
    {
        private HttpClient httpClient;
        protected JsonSerializerSettings Settings { get; set; } =  new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            MissingMemberHandling = MissingMemberHandling.Ignore,
        };

        public BaseLinnWorksClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        private void AddHeaders(
            HttpRequestMessage request, 
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers)
        {
            if(headers?.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
        }

        private void BuildRequestUri(
            HttpRequestMessage request, 
            string uri, 
            IReadOnlyDictionary<string, string> queryParameters)
        {
            var url = new StringBuilder();
            url.Append(uri);
            
            if(queryParameters?.Count > 0)
            {
                url.Append("?");
                foreach (var parameter in queryParameters)
                {
                    url.Append($"{parameter.Key}={parameter.Value}&");
                }

                url.Remove(url.Length - 1, 1);
            }
            
            request.RequestUri = new Uri(url.ToString(), UriKind.RelativeOrAbsolute);
        }

        //TODO: Works only for application/x-www-form-urlencoded content
        private void AddContent(HttpRequestMessage request, 
            string body)
        {
            if (!string.IsNullOrEmpty(body))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");;
            }
            
        }
        
        protected async Task<TResponse> SendRequestAsync<TResponse>(
            string uri,
            HttpMethod method,
            IReadOnlyDictionary<string, string> queryParameters = null, 
            string body = null,
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers = null, 
            CancellationToken cancellationToken = default)
        {
            return (await SendRequestInternalAsync<TResponse>(uri, method, queryParameters, body, headers, cancellationToken)).Object;
        }
        
        protected async Task<string> SendRequestAsync(
            string uri,
            HttpMethod method,
            IReadOnlyDictionary<string, string> queryParameters = null, 
            string body = null,
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers = null, 
            CancellationToken cancellationToken = default)
        {
            return (await SendRequestInternalAsync<bool>(uri, method, queryParameters, body, headers, cancellationToken)).Text;
        }
        
        private async Task<ObjectResponseResult<TResponse>> SendRequestInternalAsync<TResponse>(
            string uri,
            HttpMethod method,
            IReadOnlyDictionary<string, string> queryParameters = null, 
            string body = null,
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers = null, 
            CancellationToken cancellationToken = default)
        {
            EnsureArg.IsNotNullOrEmpty(uri, nameof(uri));
            
            var client = httpClient;
            using (var request = new HttpRequestMessage())
            {
                request.Method = method;
                request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse(MediaTypeNames.Application.Json));
                
                AddContent(request, body);
                AddHeaders(request, headers);
                BuildRequestUri(request, uri, queryParameters);
                
                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                try
                {
                    var responseHeaders = response.Headers.ToDictionary(h => h.Key, h => h.Value, StringComparer.OrdinalIgnoreCase);
                    if (response.Content != null && response.Content.Headers != null)
                    {
                        foreach (var item in response.Content.Headers)
                            responseHeaders[item.Key] = item.Value;
                    }

                    var status = ((int) response.StatusCode);
                    if (status >= 200 && status < 300)
                    {
                        return await ReadObjectResponseAsync<TResponse>(response, responseHeaders);
                    }
                    else
                    {
                        string responseText = (response.Content == null)
                            ? string.Empty
                            : await response.Content.ReadAsStringAsync();
                        throw new ApiException("A server side error occurred.", (int) response.StatusCode, responseText, responseHeaders, null);
                    }
                }
                finally
                {
                    response?.Dispose();
                }
            }
        }
        
        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                Object = responseObject;
                Text = responseText;
            }

            public T Object { get; }

            public string Text { get; }
        }
        
        protected  async Task<ObjectResponseResult<TResponse>> ReadObjectResponseAsync<TResponse>(
            HttpResponseMessage response,
            IReadOnlyDictionary<string,
            IEnumerable<string>> headers)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<TResponse>(default, string.Empty);
            }

            if (headers.TryGetValue("Content-Type", out var contentType) && contentType != null && contentType.Any(a => a.Contains(MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    using (var streamReader = new StreamReader(responseStream))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var serializer = JsonSerializer.Create(Settings);
                        var typedBody = serializer.Deserialize<TResponse>(jsonTextReader);
                        return new ObjectResponseResult<TResponse>(typedBody, string.Empty);
                    }
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(TResponse).FullName + ".";
                    throw new ApiException(message, (int) response.StatusCode, default, headers, exception);
                }
            }

            var responseText = await response.Content.ReadAsStringAsync();
            return new ObjectResponseResult<TResponse>(default, responseText);
        }
    }
}