using Default;
using Microsoft.OData.Client;
using System.Threading.Tasks;

namespace InLoox.ODataClient.Extensions
{
    public static class DataServiceCollectionExtension
    {
        public static async Task<bool> LoadNext<T>(this DataServiceCollection<T> collection, Container container)
        {
            if (collection.Continuation == null)
                return false;

            var list = await container.ExecuteAsync(collection.Continuation);
            collection.Load(list);
            return true;
        }

        public static async Task LoadAll<T>(this DataServiceCollection<T> collection, Container container)
        {
            while (await collection.LoadNext(container)) ;
        }
    }
}
