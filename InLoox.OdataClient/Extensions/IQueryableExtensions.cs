using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InLoox.ODataClient.Extensions
{
    public static class IQueryableExtensions
    {
        public static DataServiceQuery<T> ToDataServiceQuery<T>(this IQueryable<T> query)
        {
            var dsQuery = query as DataServiceQuery<T>;
            if (dsQuery == null)
                throw new NotSupportedException(nameof(query));
            return dsQuery;
        }

        public static Task<IEnumerable<T>> ExecuteAsync<T>(this IQueryable<T> query)
        {
            return query.ToDataServiceQuery().ExecuteAsync();
        }

        public static DataServiceContext GetContext<T>(this IQueryable<T> query)
        {
            return query.ToDataServiceQuery().Context;
        }

        public static async Task<T> FirstOrDefaultSq<T>(this IQueryable<T> query)
        {
            var list = await query.ExecuteAsync().ConfigureAwait(false);
            return list.FirstOrDefault();
        }
    }
}
