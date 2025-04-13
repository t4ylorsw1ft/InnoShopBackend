using MediatR;

namespace InnoShop.ProductService.Application.UseCases.Files.Queries.GetImage
{
    public record GetImageQuery(string FilePath) : IRequest<byte[]>;
}
