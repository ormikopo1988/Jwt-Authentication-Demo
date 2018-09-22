using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LinkedInClient.Services
{
    public class WebApiClientService : IWebApiClientService
    {
        private static readonly HttpClient _httpClientInstance = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10),
            MaxResponseContentBufferSize = 256000
        };

        public void AddRequestHeader(HttpRequestMessage request, string key, string value)
        {
            if(request != null && request.Headers != null && !string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                request.Headers.Add(key, value);
            }
        }

        public async Task<HttpResponseMessage> PostAsyncFacade(string requestUri, HttpContent content)
        {
            try
            {
                return await _httpClientInstance.PostAsync(requestUri, content);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<HttpResponseMessage> CustomGetAsync(Uri apiUri, Dictionary<string, string> requestHeaders)
        {
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = apiUri
                };

                FillRequestHeaders(request, requestHeaders);

                return await _httpClientInstance.SendAsync(request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<HttpResponseMessage> CustomPostAsync<TPostRequestBody>(TPostRequestBody item, Uri apiUri, Dictionary<string, string> requestHeaders)
        {
            try
            {
                var json = JsonConvert.SerializeObject(item);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = apiUri,
                    Content = content
                };

                FillRequestHeaders(request, requestHeaders);

                return await _httpClientInstance.SendAsync(request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void FillRequestHeaders(HttpRequestMessage request, Dictionary<string, string> requestHeaders)
        {
            if (requestHeaders != null)
            {
                foreach (var headerPair in requestHeaders)
                {
                    AddRequestHeader(request, headerPair.Key, headerPair.Value);
                }
            }
        }
    }
}