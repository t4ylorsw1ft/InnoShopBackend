namespace InnoShop.ProductService.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(byte[] fileData, string fileName, CancellationToken cancellationToken);
        Task<byte[]> GetFileAsync(string filePath, CancellationToken cancellationToken);
        bool DeleteFile(string filePath);
    }
}
