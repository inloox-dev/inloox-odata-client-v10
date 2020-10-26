using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using InLoox.ODataClient.Data.BusinessObjects;
using IQmedialab.InLoox.Data.Api.Model.OData;

namespace InLoox.ODataClient.Services
{
    public interface IDocumentService
    {
        Task<DocumentFolderView> CreateFolder(string name, Guid? projectId, Guid? parentFolder = null);
        Task DeleteFile(Guid documentId);
        Task DeleteFolder(Guid folderId);
        Task<HttpResponseMessage> DownloadDocument(Guid documentId);
        Task<IEnumerable<DocumentEntry>> GetDocumentEntries(Guid? projectId);
        Task<IEnumerable<DocumentView>> GetFiles(Guid? projectId);
        Task<IEnumerable<DocumentFolderView>> GetFolders(Guid? projectId);
        Task<IEnumerable<DocumentEntry>> GetLatestChanges(int skip, int take);
        Task<Guid?> UploadDocument(string fileName, Stream file, Guid? projectId = null, Guid? folderId = null);
    }
}