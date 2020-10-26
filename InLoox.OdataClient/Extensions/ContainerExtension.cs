using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InLoox.ODataClient.Extensions
{
    public static class ContainerExtension
    {
        public static async Task<T> PostObject<T>(this Default.Container container, string url, object obj)
        {
            var client = container.GetHttpClient();
            var request = new HttpRequestMessage(new HttpMethod("POST"), url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return default;
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
    }
}
