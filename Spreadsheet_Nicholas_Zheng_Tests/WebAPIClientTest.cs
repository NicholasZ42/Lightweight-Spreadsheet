namespace Spreadsheet_Nicholas_Zheng_Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;
    using RichardSzalay.MockHttp; // Mock HTTP Library
    using WebAPIClient;

    /// <summary>
    /// Tests all methods in the WebAPIClient class.
    /// </summary>
    [TestFixture]
    public class WebAPIClientTest
    {
        /// <summary>
        /// Test ProcessRepositoriesAsync by mocking.
        /// </summary>
        /// <returns><see cref="Task"/> Return for an async function with no return value. </returns>
        [Test]
        public async Task ProcessRepositoriesAsyncTestAsync()
        {
            string expectedResponse = @"
            [
                {
                    ""name"": ""testRepo"",
                    ""description"": ""test"",
                    ""html_url"": ""https://github.com/testRepo"",
                    ""homepage"": ""https://testRepo.com"",
                    ""watchers"": 42,
                    ""pushed_at"": ""2022-05-01T00:00:00Z""
                }
            ]";

            int expectedCount = 1;
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("https://api.github.com/orgs/dotnet/repos")
                .Respond("application/json", expectedResponse);

            var client = new HttpClient(mockHttp);

            WebClient webClient = new WebClient(client);
            var res = await webClient.ProcessRepositoriesAsync();

            Assert.IsNotNull(res);
            Assert.AreEqual(expectedCount, res.Count);

            var repository = res.First();
            Assert.AreEqual("testRepo", repository.Name);
            Assert.AreEqual("test", repository.Description);
            Assert.AreEqual(new Uri("https://github.com/testRepo"), repository.GitHubHomeUrl);
            Assert.AreEqual(new Uri("https://testRepo.com"), repository.Homepage);
            Assert.AreEqual(42, repository.Watchers);
            Assert.AreEqual(DateTime.Parse("2022-05-01T00:00:00Z").ToLocalTime(), repository.LastPush);
        }
    }
}
