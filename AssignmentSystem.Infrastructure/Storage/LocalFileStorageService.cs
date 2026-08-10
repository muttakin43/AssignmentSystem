using AssignmentSystem.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AssignmentSystem.Application.Interfaces.IFileStorageService;

namespace AssignmentSystem.Infrastructure.Storage
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _rootPath;

        public LocalFileStorageService(IConfiguration configuration)
        {
            _rootPath = configuration["FileStorage:RootPath"] ?? Path.Combine(AppContext.BaseDirectory, "uploads");
            Directory.CreateDirectory(_rootPath);
        }

        public async Task<StoredFile> SaveAsync(Stream content, string fileName, CancellationToken ct = default)
        {
            var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
            var fullPath = Path.Combine(_rootPath, safeFileName);

            await using var fileStream = new FileStream(fullPath, FileMode.Create);
            await content.CopyToAsync(fileStream, ct);

            return new StoredFile(safeFileName);
        }

        public Task<Stream> OpenReadAsync(string relativePath, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(_rootPath, relativePath);
            Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return Task.FromResult(stream);
        }

        public void Delete(string relativePath)
        {
            var fullPath = Path.Combine(_rootPath, relativePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
