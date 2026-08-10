using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Interfaces
{
    public interface IFileStorageService
    {
        public record StoredFile(string RelativePath);
        Task<StoredFile> SaveAsync(Stream content, string fileName, CancellationToken ct = default);
        Task<Stream> OpenReadAsync(string relativePath, CancellationToken ct = default);
        void Delete(string relativePath);
    }
}
