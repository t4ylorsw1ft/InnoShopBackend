using MediatR;

namespace InnoShop.ProductService.Application.UseCases.Files.Commands.UploadImage
{
    public record UploadImageCommand(byte[] FileData, string FileName) : IRequest<string>;
}
