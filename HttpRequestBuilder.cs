using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Itf.PlayersPortal.Core.Http
{
    public class HttpRequestBuilder : IHttpRequestBuilder
    {
        private readonly HttpClient _client;

        private HttpRequestMessage _request;

        private bool _configureAwait;

        public HttpRequestBuilder(HttpClient client)
        {
            _client = client;
        }

        public IHttpRequestBuilder WithRequest(HttpMethod httpMethod, Uri resource)
        {
            _request = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = resource
            };

            return this;
        }

        public IHttpRequestBuilder WithHeaders(Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                _request.Headers.Add(header.Key, header.Value);
            }
            return this;
        }

        public IHttpRequestBuilder WithHeaders(Dictionary<string, IEnumerable<string>> headers)
        {
            foreach (var header in headers)
            {
                _request.Headers.Add(header.Key, header.Value);
            }
            return this;
        }

        public IHttpRequestBuilder WithContent(object value, Encoding encoding, string mediaType = "application/json")
        {
            _request.Content = new StringContent(JsonConvert.SerializeObject(value), encoding, mediaType);
            return this;
        }

        public IHttpRequestBuilder ConfigureAwait(bool configure)
        {
            _configureAwait = configure;
            return this;
        }

        //TODO : add http retry with polly
        public async Task<HttpStatusResponse<T>> Execute<T>()
        {
            try
            {
                var response = await SendAsync();
                T deserializedObject;

                if (response.IsSuccessStatusCode)
                {
                    deserializedObject = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    // TODO: Log error
                    deserializedObject = default(T);
                }

                return new HttpStatusResponse<T>
                {
                    StatusCode = response.StatusCode,
                    Response = deserializedObject
                };
            }
            catch (Exception ex)
            {
                // TODO: Logging
                throw;
            }
        }

        private async Task<HttpResponseMessage> SendAsync()
        {
            if (_configureAwait)
            {
                return await _client.SendAsync(_request).ConfigureAwait(_configureAwait);
            }

            return await _client.SendAsync(_request);
        }
    }
}
