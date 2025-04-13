
using InnoShop.ProductService.Domain.Interfaces;
using MediatR;


namespace InnoShop.ProductService.Application.UseCases.Products.Commands.ActivateProductsByUser
{
    public class ActivateProductsByUserCommandHandler : IRequestHandler<ActivateProductsByUserCommand>
    {
        private readonly IProductRepository _productRepository;

        public ActivateProductsByUserCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task Handle(ActivateProductsByUserCommand request, CancellationToken cancellationToken)
        {
            await _productRepository.ActivateByUserIdAsync(request.UserId, cancellationToken);
        }
    }
}
