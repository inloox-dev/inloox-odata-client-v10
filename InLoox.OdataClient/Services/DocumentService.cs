using Default;
using InLoox.ODataClient.Data.BusinessObjects;
using InLoox.ODataClient.Extensions;
using IQmedialab.InLoox.Data.Api.Model.OData;
using Microsoft.OData.Client;
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
            using var requestContent = new MultipartFormDataContent
            {
                { new StringContent(projectId.ToString()), "ProjectId" },
                { new StringContent(folderId.ToString()), "DocumentFolderId" },
                { new StreamContent(file), fileName, fileName },
            };

            var (success, guid) = await UploadOrReplace(requestContent);
            return success ? guid : null;
        }

        public async Task<bool> ReplaceDocument(Guid documentId, Stream file)
        {
            using var requestContent = new MultipartFormDataContent
            {
                { new StringContent(documentId.ToString()), "DocumentId" },
                { new StreamContent(file), "file", "file" },
            };
            var (success, _) = await UploadOrReplace(requestContent);
            return success;
        }

        private async Task<(bool, Guid?)> UploadOrReplace(MultipartFormDataContent requestContent)
        {
            var url = new Uri(_ctx.BaseUri, "/file/uploadnewdocument");
            var client = _ctx.GetHttpClient();

            var resp = await client.PostAsync(url, requestContent);
            if (!resp.IsSuccessStatusCode)
                return (false, null);
            var body = await resp.Content.ReadAsStringAsync();
            var guids = JsonConvert.DeserializeObject<Guid[]>(body);
            return (true, guids?.FirstOrDefault());
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
            return await _ctx.documentview.movedocumententry(documentId, targetDocumentFolderId)
                .GetValueAsync();
        }

        public async Task RenameDocument(Guid documentId, string newName)
        {
            var document = await GetDocumentFromCollection(documentId);
            document.FileName = newName;
            await _ctx.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
        }

        public async Task RenameFolder(Guid folderId, string newName)
        {
            var folder = await GetFolderFromCollection(folderId);
            folder.FolderName = newName;
            await _ctx.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
        }

        public Task<HttpResponseMessage> DownloadDocument(Guid documentId)
        {
            var url = new Uri(_ctx.BaseUri, "/file/downloaddocument/" + documentId);
            var client = _ctx.GetHttpClient();
            return client.GetAsync(url);
        }

        public async Task<DocumentView> GetDocumentFromCollection(Guid documentId)
        {
            var documentQuery = _ctx.documentview.Where(k => k.DocumentId == documentId);
            var documentCollection = await ODataBasics.GetDSCollection(documentQuery);
            return documentCollection.FirstOrDefault();
        }

        public async Task<DocumentFolderView> GetFolderFromCollection(Guid folderId)
        {
            var folderQuery = _ctx.documentfolderview.Where(k => k.DocumentFolderId == folderId);
            var folderCollection = await ODataBasics.GetDSCollection(folderQuery);
            return folderCollection.FirstOrDefault();
        }
    }
}
