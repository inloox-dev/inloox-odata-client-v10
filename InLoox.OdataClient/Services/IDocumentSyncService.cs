using IQmedialab.InLoox.Data.Api.Model.OData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InLoox.ODataClient.Services
{
    public interface IDocumentSyncService
    {
        Task<IEnumerable<DocumentSync>> GetDocumentEntries(Guid? projectId);
        Task<IEnumerable<DocumentSync>> GetLatestChanges(int skip, int take);
    }
}