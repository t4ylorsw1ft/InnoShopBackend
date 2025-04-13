using InnoShop.ProductService.Application.Interfaces.Services;
using MediatR;

namespace InnoShop.ProductService.Application.UseCases.Files.Queries.GetImage
{
    public class GetImageQueryHandler : IRequestHandler<GetImageQuery, byte[]>
    {
        private readonly IFileStorageService _fileStorageService;

        public GetImageQueryHandler(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task<byte[]> Handle(GetImageQuery request, CancellationToken cancellationToken)
        {
            return await _fileStorageService.GetFileAsync(request.FilePath, cancellationToken);
        }
    }
}
