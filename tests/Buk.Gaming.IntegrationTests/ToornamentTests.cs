using System;
using System.Threading.Tasks;
using Xunit;

namespace Buk.Gaming.IntegrationTests
{
    public class ToornamentTests : IntegrationTestBase
    {
        [Fact]
        public async Task Toornament_AuthenticateWithClientCredentials_ShouldGetAccessToken()
        {
            var server = CreateTestServer();
            var client = server.CreateClient();

            var response = await client.GetAsync("api/tournaments");
            var content = await response.Content.ReadAsStringAsync();

            Assert.True(response.IsSuccessStatusCode);

        }
    }
}
