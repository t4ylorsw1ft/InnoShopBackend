using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool IsAvaliable { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; }
    }
}
