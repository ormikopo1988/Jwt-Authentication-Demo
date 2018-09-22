using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinkedInClient.Services
{
    public interface IWebApiClientService
    {
        void AddRequestHeader(HttpRequestMessage request, string key, string value);

        Task<HttpResponseMessage> PostAsyncFacade(string requestUri, HttpContent content);

        Task<HttpResponseMessage> CustomGetAsync(Uri apiUri, Dictionary<string, string> requestHeaders);

        Task<HttpResponseMessage> CustomPostAsync<TPostRequestBody>(TPostRequestBody item, Uri apiUri, Dictionary<string, string> requestHeaders);
    }
}