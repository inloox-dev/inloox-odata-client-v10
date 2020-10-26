using Default;
using InLoox.ODataClient.Extensions;
using IQmedialab.InLoox.Data.Api.Model.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InLoox.ODataClient.Services
{
    public class DocumentSyncService : IDocumentSyncService
    {
        private readonly Container _ctx;

        public DocumentSyncService(Container ctx)
        {
            _ctx = ctx;
        }

        public Task<IEnumerable<DocumentSync>> GetDocumentEntries(Guid? projectId)
        {
            var query = _ctx.documentsync.Where(k => k.ProjectId == projectId)
                .ToDataServiceQuery();

            return query.ExecuteAsync();
        }

        public Task<IEnumerable<DocumentSync>> GetLatestChanges(int skip, int take)
        {
            var query = _ctx.documentsync.OrderByDescending(k => k.UpdatedAt)
                .Skip(skip)
                .Take(take)
                .ToDataServiceQuery();

            return query.ExecuteAsync();
        }
    }
}