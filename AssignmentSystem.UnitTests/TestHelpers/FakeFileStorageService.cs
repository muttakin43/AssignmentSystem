using AssignmentSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AssignmentSystem.Application.Interfaces.IFileStorageService;

namespace AssignmentSystem.UnitTests.TestHelpers
{
    public class FakeFileStorageService : IFileStorageService
    {
        public Task<StoredFile> SaveAsync(Stream content, string fileName, CancellationToken ct = default)
            => Task.FromResult(new StoredFile($"fake/{fileName}"));

        public Task<Stream> OpenReadAsync(string relativePath, CancellationToken ct = default)
            => Task.FromResult<Stream>(new MemoryStream());

        public void Delete(string relativePath) { }
    }
}
