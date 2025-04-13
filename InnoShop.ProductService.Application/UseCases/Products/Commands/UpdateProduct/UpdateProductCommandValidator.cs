using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Application.UseCases.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator() 
        {
            RuleFor(p => p.Name)
                    .NotEmpty().WithMessage("Product name is required")
                    .MaximumLength(100);

            RuleFor(p => p.Description)
                    .NotEmpty().WithMessage("Description is required")
                    .MaximumLength(1000);

            RuleFor(p => p.Price)
                    .GreaterThan(0).WithMessage("Price must be greater than 0")
                    .LessThanOrEqualTo(999999).WithMessage("Price must be less than 999999");
        }
    }
}
