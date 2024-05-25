using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPIClient
{
    public class WebClient
    {
        HttpClient client;
        public WebClient(HttpClient client) 
        {
            this.client = client;
        }
        public async Task<List<Repository>> ProcessRepositoriesAsync()
        {
            await using Stream stream =
                await client.GetStreamAsync("https://api.github.com/orgs/dotnet/repos");
            var repositories =
                await JsonSerializer.DeserializeAsync<List<Repository>>(stream);
            return repositories ?? new();
        }
    }
}
