using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Itf.PlayersPortal.Core.Http
{
    public interface IHttpRequestBuilder
    {
        IHttpRequestBuilder WithRequest(HttpMethod httpMethod, Uri resource);

        IHttpRequestBuilder WithHeaders(Dictionary<string, string> headers);

        IHttpRequestBuilder WithHeaders(Dictionary<string, IEnumerable<string>> headers);

        IHttpRequestBuilder ConfigureAwait(bool configure);

        IHttpRequestBuilder WithContent(object value, Encoding encoding, string mediaType = "application/json");

        Task<HttpStatusResponse<T>> Execute<T>();
    }
}
