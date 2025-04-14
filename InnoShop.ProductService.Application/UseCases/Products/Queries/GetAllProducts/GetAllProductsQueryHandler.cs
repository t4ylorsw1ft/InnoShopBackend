using AutoMapper;
using InnoShop.ProductService.Application.UseCases.Products.DTOs;
using InnoShop.ProductService.Domain.Entities;
using InnoShop.ProductService.Domain.Interfaces;
using MediatR;

namespace InnoShop.ProductService.Application.UseCases.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductLookupDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductLookupDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var users = await _productRepository.GetAllAsync(
                request.Name,
                request.MinPrice,
                request.MaxPrice,
                request.IsAvailable,
                request.UserId,
                cancellationToken
            );
            return _mapper.Map<List<ProductLookupDto>>(users);
        }
    }

}
