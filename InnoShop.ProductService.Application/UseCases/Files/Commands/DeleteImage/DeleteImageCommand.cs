using MediatR;

namespace InnoShop.ProductService.Application.UseCases.Files.Commands.DeleteImage
{
    public record DeleteImageCommand(string FilePath) : IRequest;
}
