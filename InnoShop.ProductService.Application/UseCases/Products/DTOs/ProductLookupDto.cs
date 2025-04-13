using AutoMapper;
using InnoShop.ProductService.Application.Common.Mapping;
using InnoShop.ProductService.Domain.Entities;

namespace InnoShop.ProductService.Application.UseCases.Products.DTOs
{
    public class ProductLookupDto : IMapWith<Product>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool IsAvaliable { get; set; }
        public string? ImagePath { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductLookupDto>();
        }
    }
}
