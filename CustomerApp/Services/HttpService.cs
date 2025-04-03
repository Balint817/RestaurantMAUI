using CustomerApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net;
using System.Globalization;

namespace CustomerApp.Services
{
    public sealed class HttpService: Singleton<HttpService>
    {
        static HttpService? _instance;
        public static HttpService Instance => _instance ??= new();

        private readonly HttpClient _httpClient;
        public string BaseAPIUrl => "https://mateszadam.koyeb.app";
        private HttpService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
            //_httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new(CultureInfo.CurrentCulture.Name));
            _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new("hu-HU"));
        }

        public void Authorize(string token)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(token, nameof(token));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        internal string? Unauthorize()
        {
            var token = _httpClient.DefaultRequestHeaders.Authorization?.Parameter;
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return token;
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _httpClient.GetAsync(url);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent? data)
        {
            return await _httpClient.PostAsync(url, data);
        }
        public async Task<HttpResponseMessage> PostJsonAsync(string url, object obj)
        {
            return await _httpClient.PostAsJsonAsync(url, obj);
        }

        public async Task<HttpResponseMessage> PutAsync(string url, HttpContent? data)
        {
            return await _httpClient.PutAsync(url, data);
        }
        public async Task<HttpResponseMessage> PutJsonAsync(string url, object obj)
        {
            return await _httpClient.PutAsJsonAsync(url, obj);
        }

        public async Task<HttpResponseMessage> PatchAsync(string url, HttpContent? data)
        {
            return await _httpClient.PatchAsync(url, data);
        }
        public async Task<HttpResponseMessage> PatchJsonAsync(string url, object obj)
        {
            return await _httpClient.PatchAsJsonAsync(url, obj);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await _httpClient.DeleteAsync(url);
        }

        public async Task<HttpResponseMessage> SendCustomAsync(HttpRequestMessage message)
        {
            return await _httpClient.SendAsync(message);
        }

        public string FormatQuery(string baseUrl, Dictionary<string, object?> queryParams)
        {
            if (queryParams.Count == 0)
            {
                return baseUrl;
            }
            var queryParamsArray = queryParams.ToArray();
            var sb = new StringBuilder();
            sb.Append(baseUrl);

            var firstParam = queryParamsArray[0];
            sb.Append('?');

            ArgumentException.ThrowIfNullOrEmpty(firstParam.Key, nameof(queryParams));
            sb.Append(WebUtility.UrlEncode(firstParam.Key));
            sb.Append('=');
            sb.Append(WebUtility.UrlEncode(firstParam.Value?.ToString() ?? ""));

            for (int i = 1; i < queryParamsArray.Length; i++)
            {
                var param = queryParamsArray[i];
                sb.Append('&');

                ArgumentException.ThrowIfNullOrEmpty(param.Key, nameof(queryParams));
                sb.Append(WebUtility.UrlEncode(param.Key));
                sb.Append('=');
                sb.Append(WebUtility.UrlEncode(param.Value?.ToString() ?? ""));
            }

            return sb.ToString();
        }

        public async Task<T[]> GetAllPagedItemsAsync<T>(string baseUrl, Dictionary<string, object?> baseQueryParams)
        {
            var allItems = new List<T>();
            int currentPage = 1;
            int pageCount = 1;

            while (currentPage <= pageCount)
            {
                var queryParams = new Dictionary<string, object?>(baseQueryParams)
                {
                    { "page", currentPage },
                    { "limit", 50 }
                };

                string url = FormatQuery(baseUrl, queryParams);
                HttpResponseMessage response = await GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return [];
                    }
                    throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
                }

                var result = await response.Content.ReadFromJsonAsync<PagedResponse<T>>()
                    ?? throw new InvalidOperationException("Invalid response format");

                allItems.AddRange(result.items);
                pageCount = result.pageCount;
                currentPage++;
            }

            return allItems.ToArray();
        }

        public class PagedResponse<T>
        {
            public int pageCount { get; set; }
            public List<T> items { get; set; } = [];
        }


    }
}
