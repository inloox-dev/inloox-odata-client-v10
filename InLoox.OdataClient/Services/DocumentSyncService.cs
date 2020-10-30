using Default;
using InLoox.ODataClient.Extensions;
using InLoox.ODataClient.Services.Enums;
using IQmedialab.InLoox.Data.Api.Model.OData;
using Microsoft.OData.Client;
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

        public Task<DataServiceCollection<DocumentSync>> GetChangesSince(DateTimeOffset sinceDate,
            DocumentSyncChangeType changeType = DocumentSyncChangeType.All)
        {
            var query = _ctx.documentsync
                    .OrderByDescending(k => k.UpdatedAt)
                    .Where(k => k.UpdatedAt > sinceDate);

            switch (changeType)
            {
                case DocumentSyncChangeType.File:
                    query = query.Where(k => !k.IsFolder);
                    break;
                case DocumentSyncChangeType.Folders:
                    query = query.Where(k => k.IsFolder);
                    break;
            }

            return ODataBasics.GetDSCollection(query);
        }
    }
}