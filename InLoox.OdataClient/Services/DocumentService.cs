using Default;
using InLoox.ODataClient.Data.BusinessObjects;
using InLoox.ODataClient.Extensions;
using IQmedialab.InLoox.Data.Api.Model.OData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace InLoox.ODataClient.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly Container _ctx;

        public DocumentService(Container ctx)
        {
            _ctx = ctx;
        }

        private class SerializeableFolder
        {
            public string FolderName { get; set; }
            public Guid? ProjectId { get; set; }
            public Guid? ParentDocumentFolderId { get; set; }
        }

        public Task<DocumentFolderView> CreateFolder(string name, Guid? projectId, Guid? parentFolder = null)
        {
            var folder = new SerializeableFolder
            {
                ProjectId = projectId,
                FolderName = name,
                ParentDocumentFolderId = parentFolder
            };

            return _ctx.PostObject<DocumentFolderView>(_ctx.documentfolderview.RequestUri.ToString(), folder);
        }

        public async Task DeleteFolder(Guid folderId)
        {
            var folder = await _ctx.documentfolderview
                .Where(k => k.DocumentFolderId == folderId && k.IsArchived == false)
                .FirstOrDefaultSq();

            _ctx.DeleteObject(folder);

            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteFile(Guid documentId)
        {
            var folder = await _ctx.documentview
                .Where(k => k.DocumentId == documentId && k.IsArchived == false)
                .FirstOrDefaultSq();

            _ctx.DeleteObject(folder);

            await _ctx.SaveChangesAsync();
        }

        public Task<IEnumerable<DocumentFolderView>> GetFolders(Guid? projectId)
        {
            var query = _ctx.documentfolderview.Where(k => k.ProjectId == projectId)
                .ToDataServiceQuery();

            return query.ExecuteAsync();
        }

        public Task<IEnumerable<DocumentView>> GetFiles(Guid? projectId)
        {
            var query = _ctx.documentview.Where(k => k.ProjectId == projectId)
                .ToDataServiceQuery();

            return query.ExecuteAsync();
        }

        public Task<IEnumerable<DocumentEntry>> GetDocumentEntries(Guid? projectId)
        {
            var query = _ctx.documententry.Where(k => k.ProjectId == projectId)
                .ToDataServiceQuery();

            return query.ExecuteAsync();
        }

        public async Task<Guid?> UploadDocument(string fileName, Stream file, Guid? projectId = null, Guid? folderId = null)
        {
            var url = new Uri(_ctx.BaseUri, "/file/uploadnewdocument");
            var client = _ctx.GetHttpClient();

            using var requestContent = new MultipartFormDataContent
            {
                { new StringContent(projectId.ToString()), "ProjectId" },
                { new StringContent(folderId.ToString()), "DocumentFolderId" },
                { new StreamContent(file), fileName, fileName },
            };

            var resp = await client.PostAsync(url, requestContent);
            if (!resp.IsSuccessStatusCode)
                return null;
            var body = await resp.Content.ReadAsStringAsync();
            var guids = JsonConvert.DeserializeObject<Guid[]>(body);
            return guids.Any() ? (Guid?)guids.First() : null;
        }

        public Task<IEnumerable<DocumentEntry>> GetLatestChanges(int skip, int take)
        {
            var query = _ctx.documententry.OrderByDescending(k => k.ChangedDate)
                .Skip(skip)
                .Take(take)
                .ToDataServiceQuery();

            return query.ExecuteAsync();
        }

        public async Task<bool> MoveDocument(Guid documentId, Guid targetDocumentFolderId)
        {
            return await _ctx.documentview.movedocumententry(documentId, targetDocumentFolderId).GetValueAsync();
        }

        public Task<HttpResponseMessage> DownloadDocument(Guid documentId)
        {
            var url = new Uri(_ctx.BaseUri, "/file/downloaddocument/" + documentId);
            var client = _ctx.GetHttpClient();

            return client.GetAsync(url);
        }
    }
}
