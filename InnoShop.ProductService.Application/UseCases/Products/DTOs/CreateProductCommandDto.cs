using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Application.UseCases.Products.DTOs
{
    public class CreateProductCommandDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string? ImagePath { get; set; }
    }
}
