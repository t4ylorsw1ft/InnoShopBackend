using InnoShop.ProductService.Domain.Interfaces;
using MediatR;

namespace InnoShop.ProductService.Application.UseCases.Products.Commands.DeactivateProductsByUser
{
    public class DeactivateProductsByUserCommandHandler : IRequestHandler<DeactivateProductsByUserCommand>
    {
        private readonly IProductRepository _productRepository;

        public DeactivateProductsByUserCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task Handle(DeactivateProductsByUserCommand request, CancellationToken cancellationToken)
        {
            await _productRepository.DeactivateByUserIdAsync(request.UserId, cancellationToken);
        }
    }

}
