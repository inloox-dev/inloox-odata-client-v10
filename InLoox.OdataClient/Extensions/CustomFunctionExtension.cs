using IQmedialab.InLoox.Data.Api.Model.OData;
using Microsoft.OData.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InLoox.ODataClient.Extensions
{
    public static class CustomFunctionExtension
    {
        public static async Task<IEnumerable<CustomExpandExtend>> GetCustomExpand(this DataServiceQuery<CustomExpandExtend> source, Guid objectid, HttpClient httpClient = null)
        {
            var getCustomExpandFunc = source.CreateFunctionQuery<IEnumerable<CustomExpandExtend>>("Default.getcustomexpand", false, new UriOperationParameter("objectid", objectid));
            var client = httpClient ?? (getCustomExpandFunc.Context as Default.Container).GetHttpClient(true);
            var data = await client.GetStringAsync(getCustomExpandFunc.RequestUri);
            return JsonConvert.DeserializeObject<IEnumerable<CustomExpandExtend>>(data);
        }

        public static async Task<CustomExpandExtend> PatchCustomExpand(this DataServiceQuery<CustomExpandExtend> source, Guid customExpandId, string property, string value, HttpClient httpClient = null)
        {
            var byKey = source.ByKey(customExpandId);
            var client = httpClient ?? (byKey.Context as Default.Container).GetHttpClient(false);
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), byKey.RequestUri);
            request.Content = new StringContent($@"{{""{property}"":""{value}""}}", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            return JsonConvert.DeserializeObject<CustomExpandExtend>(await response.Content.ReadAsStringAsync());
        }
    }
}
