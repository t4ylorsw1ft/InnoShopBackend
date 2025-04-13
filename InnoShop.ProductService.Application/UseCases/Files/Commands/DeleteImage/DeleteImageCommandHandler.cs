using InnoShop.ProductService.Application.Interfaces.Services;
using MediatR;

namespace InnoShop.ProductService.Application.UseCases.Files.Commands.DeleteImage
{
    public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand>
    {
        private readonly IFileStorageService _fileStorageService;

        public DeleteImageCommandHandler(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task Handle(DeleteImageCommand request, CancellationToken cancellationToken)
        {
            if (!_fileStorageService.DeleteFile(request.FilePath))
                throw new FileNotFoundException(request.FilePath + " not found");
        }
    }
}
