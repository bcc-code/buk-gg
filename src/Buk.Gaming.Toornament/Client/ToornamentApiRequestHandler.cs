using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Toornament
{
    public class ToornamentApiRequestHandler
    {
        protected HttpClient Client { get; set; }
        protected string UriPrefix { get; set; }
        public JsonSerializerSettings SerializationSettings { get; set; }

        public ToornamentApiRequestHandler(HttpClient client, string uriPrefix)
        {
            Client = client;
            UriPrefix = uriPrefix;
            SerializationSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
        }


        protected virtual TResponse ParseContent<TResponse>(string content)
        {
            if (content == null)
            {
                return default(TResponse);
            }
            if (typeof(TResponse) == typeof(string))
            {
                return (TResponse)(object)content;
            }
            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        protected virtual string GetQueryStringFromObject(object parameters)
        {
            if (parameters == null || parameters.ToString() == "")
            {
                return "";
            }
            var type = parameters.GetType();
            if (type.GetTypeInfo().IsPrimitive)
            {
                throw new ArgumentException("Parameters must be an object with fields or properties. Primative types are not supported.");
            }
            var output = new StringBuilder("");
            foreach (var field in type.GetTypeInfo().GetFields(System.Reflection.BindingFlags.Public))
            {
                var rawValue = field.GetValue(parameters);
                string value = null;
                if (rawValue != null && rawValue.GetType() == typeof(DateTime))
                {
                    value = ((DateTime)rawValue).ToString("yyyy-MM-dd");
                }
                else if (rawValue != null && rawValue.GetType() == typeof(DateTimeOffset))
                {
                    value = ((DateTimeOffset)rawValue).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                }
                else
                {
                    value = rawValue?.ToString();
                }

                if (!string.IsNullOrEmpty(value))
                {
                    output.Append($"{field.Name}={WebUtility.UrlEncode(value)}&");
                }
            }
            foreach (var property in type.GetTypeInfo().GetProperties())
            {
                var rawValue = property.GetValue(parameters);
                string value = null;
                if (rawValue != null && rawValue.GetType() == typeof(DateTime))
                {
                    value = ((DateTime)rawValue).ToString("yyyy-MM-dd");
                }
                else if (rawValue != null && rawValue.GetType() == typeof(DateTimeOffset))
                {
                    value = ((DateTimeOffset)rawValue).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                }
                else
                {
                    value = rawValue?.ToString();
                }
                if (!string.IsNullOrEmpty(value))
                {
                    output.Append($"{property.Name}={WebUtility.UrlEncode(value)}&");
                }
            }
            var result = output.ToString().TrimEnd('&');
            return result.Length > 0 ? $"?{result}" : "";
        }

        protected virtual async Task<TResponse> HandleResponseErrorAsync<TResponse>(HttpResponseMessage response, string relativeUri)
        {
            var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var path = Client.BaseAddress + $"{UriPrefix}/{relativeUri}".TrimEnd('/');
            throw new ToornamentApiException($"Request to {path} failed with status code {response.StatusCode}.{(!string.IsNullOrEmpty(errorContent) ? " Response: " + errorContent : "")}")
            {
                StatusCode = response.StatusCode,
                RequestUri = path,
                Response = errorContent ?? ""
            };
        }

        public Task<TResponse> GetAsync<TResponse>(object parameters)
        {
            return GetAsync<TResponse>("", "", parameters);
        }

        public async Task<TResponse> GetAsync<TResponse>(string relativeUri, string unit, object parameters = null, int take = 50, int skip = 0)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{UriPrefix}/{relativeUri}{GetQueryStringFromObject(parameters)}".TrimEnd('/'));
            if (!string.IsNullOrEmpty(unit))
            {
                request.Headers.Add("Range", $"{unit?.ToLower() ?? ""}={skip}-{skip + take - 1}");
            }

            var response = await Client.SendAsync(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return ParseContent<TResponse>(content);
            }
            else
            {
                return await HandleResponseErrorAsync<TResponse>(response, relativeUri).ConfigureAwait(false);
            }
        }

        public async Task<TResponse> GetSingleAsync<TResponse>(string relativeUri, object parameters = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{UriPrefix}/{relativeUri}{GetQueryStringFromObject(parameters)}".TrimEnd('/'));

            var response = await Client.SendAsync(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return ParseContent<TResponse>(content);
            }
            else
            {
                return await HandleResponseErrorAsync<TResponse>(response, relativeUri).ConfigureAwait(false);
            }
        }

        public Task<TResponse> GetFirstOrDefaultAsync<TResponse>(object parameters)
        {
            return GetFirstOrDefaultAsync<TResponse>("", "", parameters);
        }

        public async Task<TResponse> GetFirstOrDefaultAsync<TResponse>(string relativeUri, string unit, object parameters = null)
        {
            var response = await GetAsync<List<TResponse>>(relativeUri, unit, parameters).ConfigureAwait(false);
            return response != null ? response.FirstOrDefault() : default(TResponse);
        }

        public virtual Task<T> PostAsync<T>(T entity = default(T))
        {
            return PostAsync<T, T>("", entity);
        }

        public virtual Task<TEntity> PostAsync<TEntity>(string relativeUri, TEntity entity = default(TEntity))
        {
            return PostAsync<TEntity, TEntity>(relativeUri, entity);
        }
        public virtual async Task<TResponse> PostAsync<TResponse, TRequest>(string relativeUri, TRequest entity = default(TRequest))
        {
            var postContent = JsonConvert.SerializeObject(entity, SerializationSettings);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{UriPrefix}/{relativeUri}".TrimEnd('/'));
            request.Content = entity == null ? new StringContent("") : new StringContent(postContent, Encoding.UTF8, "application/json");

            var response = await Client.SendAsync(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return ParseContent<TResponse>(content);
            }
            else
            {
                return await HandleResponseErrorAsync<TResponse>(response, relativeUri).ConfigureAwait(false);
            }
        }

        public virtual async Task<TResponse> PatchAsync<TResponse, TRequest>(string relativeUri, TRequest entity = default(TRequest))
        {
            var patchContent = JsonConvert.SerializeObject(entity, SerializationSettings);
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{UriPrefix}/{relativeUri}".TrimEnd('/'));
            request.Content = entity == null ? new StringContent("") : new StringContent(patchContent, Encoding.UTF8, "application/json");

            var response = await Client.SendAsync(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return ParseContent<TResponse>(content);
            }
            else
            {
                return await HandleResponseErrorAsync<TResponse>(response, relativeUri).ConfigureAwait(false);
            }
        }


        public virtual Task<TEntity> PutAsync<TEntity>(string relativeUri, TEntity entity = default(TEntity))
        {
            return PutAsync<TEntity, TEntity>(relativeUri, entity);
        }
        public virtual async Task<TResponse> PutAsync<TResponse, TRequest>(string relativeUri, TRequest entity = default(TRequest))
        {
            var postContent = JsonConvert.SerializeObject(entity, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var request = new HttpRequestMessage(HttpMethod.Put, $"{UriPrefix}/{relativeUri}".TrimEnd('/'));
            request.Content = entity == null ? new StringContent("") : new StringContent(postContent, Encoding.UTF8, "application/json");

            var response = await Client.SendAsync(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return ParseContent<TResponse>(content);
            }
            else
            {
                return await HandleResponseErrorAsync<TResponse>(response, relativeUri).ConfigureAwait(false);
            }
        }

        public virtual async Task<TResponse> DeleteAsync<TResponse>(string relativeUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{UriPrefix}/{relativeUri}".TrimEnd('/'));

            var response = await Client.SendAsync(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return ParseContent<TResponse>(content);
            }
            else
            {
                return await HandleResponseErrorAsync<TResponse>(response, relativeUri).ConfigureAwait(false);
            }
        }
    }
}
