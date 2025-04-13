using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using InnoShop.ProductService.Application.Interfaces.Services;

namespace InnoShop.ProductService.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _uploadPath;
        private readonly IMemoryCache _cache;

        public FileStorageService(IWebHostEnvironment env, IMemoryCache cache)
        {
            _uploadPath = Path.Combine(env.WebRootPath, "uploads");
            _cache = cache;

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<string> SaveFileAsync(byte[] fileData, string fileName, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(_uploadPath, fileName);
            await File.WriteAllBytesAsync(filePath, fileData, cancellationToken);
            return fileName;
        }

        public async Task<byte[]> GetFileAsync(string filePath, CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(filePath, out byte[] cachedFile))
            {
                return cachedFile;
            }

            var fullPath = Path.Combine(_uploadPath, filePath);
            var fileData = await File.ReadAllBytesAsync(fullPath, cancellationToken);

            _cache.Set(filePath, fileData, TimeSpan.FromMinutes(10));
            return fileData;
        }

        public bool DeleteFile(string filePath)
        {
            var fullPath = Path.Combine(_uploadPath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            else
                return false;
        }
    }
}
