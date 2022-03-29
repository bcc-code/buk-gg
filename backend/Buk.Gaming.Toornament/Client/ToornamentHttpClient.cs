using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Buk.Gaming.Toornament
{
    public class ToornamentHttpClient : HttpClient
    {
        public ToornamentHttpClient(ToornamentOptions options, string scopes) : base(new AuthenticationRequestHandler(options.ApiKey, options.ClientId, options.ClientSecret, scopes))
        {
        }

        public class AuthenticationRequestHandler : DelegatingHandler
        {
            public string Authority { get; set; }

            public string Scopes { get; protected set; }

            public string ApiKey { get; protected set; }

            public string ClientId { get; protected set; }

            protected string ClientSecret { get; set; }

            protected CacheKeyConfig CacheKeys { get; set; }

            public AuthenticationRequestHandler(string apiKey, string clientId, string clientSecret, string scopes, HttpMessageHandler innerHandler)
                : this(apiKey, clientId, clientSecret, scopes)
            {
                InnerHandler = innerHandler;
            }

            public AuthenticationRequestHandler(string apiKey, string clientId, string clientSecret, string scopes)
            {
                InnerHandler = new HttpClientHandler();

                if (string.IsNullOrWhiteSpace(clientId))
                {
                    throw new ArgumentException("ClientId must be specified. This should be set to the OAuth 2.0 client ID of the calling application.", "clientId");
                }
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    throw new ArgumentException("Client Secret must be specified. This should be set to the OAuth 2.0 client secret of the calling application.", "clientSecret");
                }
                if (scopes == null || scopes.Length == 0)
                {
                    throw new ArgumentException("One or more scopes must be specified.", "scopes");
                }

                Scopes = scopes;
                ApiKey = apiKey;
                ClientId = clientId;
                ClientSecret = clientSecret;
                CacheKeys = new CacheKeyConfig(ClientId, Scopes);
            }


            #region Request Handler

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var accessToken = (await GetTokenAsync().ConfigureAwait(false)).Access_token;
                request.Headers.Add("X-Api-Key", ApiKey);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    //Clear token cache in case token has expired
                    //Note: no automatic retry since we do not know the exact reason why client is unauthorized (i.e. expired token, disabled client, invalid claims etc)
                    RemoveCache(CacheKeys.Token);
                    RemoveCache(CacheKeys.TokenExpiry);
                }
                return response;
            }

            #endregion

            #region Cache
            private static ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();

            protected class CacheKeyConfig
            {
                public CacheKeyConfig(string clientId, string scopes)
                {
                    Endpoint = $"Endpoints";
                    Token = $"Token_{clientId}_{scopes}";
                    TokenExpiry = $"Expiry_{clientId}_{scopes}";
                }

                public string Endpoint { get; set; }

                public string Token { get; set; }

                public string TokenExpiry { get; set; }

                public string[] All
                {
                    get
                    {
                        return new string[] { Endpoint, Token, TokenExpiry };
                    }
                }
            }

            protected T GetCache<T>(string key)
            {
                if (_cache.ContainsKey(key))
                {
                    var value = _cache[key];
                    if (value != null)
                    {
                        return (T)_cache[key];
                    }
                }
                return default(T);
            }
            protected bool TryGetCache<T>(string key, out T value)
            {
                value = GetCache<T>(key);
                return value != null && !value.Equals(default(T));
            }

            public void SetCache(string key, object value)
            {
                _cache[key] = value;
            }

            public void RemoveCache(string key)
            {
                object val;
                _cache.TryRemove(key, out val);
            }

            public void ClearCache()
            {
                foreach (var key in CacheKeys.All)
                {
                    RemoveCache(key);
                }
            }

            #endregion


            #region Token

            protected class TokenResponse
            {
                public string Access_token { get; set; }

                public long Expires_in { get; set; }

                public string Token_type { get; set; }
            }

            protected static HttpClient _tokenRequestClient = new HttpClient();

            protected async Task<TokenResponse> GetTokenAsync()
            {
                const int EXPIRY_OFFSET = 30; //Compensate for time required to request token
                TokenResponse token;
                DateTimeOffset expiry;
                if (!TryGetCache(CacheKeys.Token, out token) ||
                    !TryGetCache(CacheKeys.TokenExpiry, out expiry) ||
                    expiry < DateTimeOffset.Now.AddSeconds(-EXPIRY_OFFSET))
                {
                    try
                    {
                        var response = await _tokenRequestClient.PostAsync("https://api.toornament.com/oauth/v2/token",
                            new StringContent($@"grant_type=client_credentials&client_id={Uri.EscapeDataString(ClientId)}&client_secret={Uri.EscapeDataString(ClientSecret)}&scope={Uri.EscapeDataString(Scopes)}",
                            Encoding.UTF8, "application/x-www-form-urlencoded")).ConfigureAwait(false);

                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        

                        if (!response.IsSuccessStatusCode)
                        {
                            ClearCache();
                            throw new Exception($"Failed to retrieve token from authorization server. Status: {response.StatusCode} {response.ReasonPhrase ?? ""}. Content: {content ?? ""}");
                        }

                        token = JsonConvert.DeserializeObject<TokenResponse>(content);

                        expiry = DateTimeOffset.Now.AddSeconds(token.Expires_in);
                        SetCache(CacheKeys.TokenExpiry, expiry);
                        SetCache(CacheKeys.Token, token);
                    }
                    catch
                    {
                        ClearCache();
                        throw;
                    }

                }
                return token;
            }

            #endregion


        }
    }
}
