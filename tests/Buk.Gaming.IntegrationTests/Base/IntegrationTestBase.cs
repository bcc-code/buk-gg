using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Buk.Gaming.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected TestServer Server { get; set; }

        public IntegrationTestBase()
        {
            Server = CreateTestServer();
        }

        protected virtual TestServer CreateTestServer()
        {
            return new TestServer(WebHost.CreateDefaultBuilder().UseStartup<TestStartup>());
        }

        protected virtual HttpClient CreateTestClient(params string[] scopes)
        {
            if (Server == null)
            {
                Server = CreateTestServer();
            }
            var httpClient = Server.CreateClient();
            // Simulate token scopes / claims using custom header
            foreach (var scope in scopes)
            {
                httpClient.DefaultRequestHeaders.Add("X-AUTH-CLAIM", $"scope:{scope}");
            }
            return httpClient;
        }


    }
}
