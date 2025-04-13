using InnoShop.ProductService.Application.UseCases.Products.Commands.ActivateProductsByUser;
using InnoShop.ProductService.Application.UseCases.Products.Commands.CreateProduct;
using InnoShop.ProductService.Application.UseCases.Products.Commands.DeactivateProductsByUser;
using InnoShop.ProductService.Application.UseCases.Products.Commands.DeleteProduct;
using InnoShop.ProductService.Application.UseCases.Products.Commands.UpdateProduct;
using InnoShop.ProductService.Application.UseCases.Products.DTOs;
using InnoShop.ProductService.Application.UseCases.Products.Queries.GetAllProducts;
using InnoShop.ProductService.Application.UseCases.Products.Queries.GetProductById;
using InnoShop.ProductService.Domain.Entities;
using InnoShop.ProductService.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.ProductService.Presentation.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [RequireActiveUser]
        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] CreateProductCommandDto productDto, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirst("userId")?.Value);
            var command = new CreateProductCommand
            (
                productDto.Name,
                productDto.Description,
                productDto.Price,
                productDto.ImagePath,
                userId
            );
            var product = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { product.Id }, product);
        }

        [Authorize]
        [RequireActiveUser]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirst("userId")?.Value);
            await _mediator.Send(new DeleteProductCommand(id, userId), cancellationToken);
            return Ok();
        }

        [Authorize]
        [RequireActiveUser]
        [HttpPut]
        public async Task<ActionResult<Product>> Update([FromBody] UpdateProductCommandDto productDto, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirst("userId")?.Value);
            var command = new UpdateProductCommand
            (
                productDto.Id,
                productDto.Name,
                productDto.Description,
                productDto.Price,
                productDto.IsAvailable,
                productDto.ImagePath,
                userId
            );
            var updatedProduct = await _mediator.Send(command, cancellationToken);
            return Ok(updatedProduct);
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductLookupDto>>> GetAll([FromQuery] string? name, [FromQuery] int? minPrice,
            [FromQuery] int? maxPrice, [FromQuery] bool? isAvailable, [FromQuery] Guid? userId
            , CancellationToken cancellationToken)
        {
            var query = new GetAllProductsQuery(name, minPrice, maxPrice, isAvailable, userId);
            var products = await _mediator.Send(query, cancellationToken);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
            return Ok(product);
        }

        [HttpPut("deactivate-user/{userId}")]
        public async Task<ActionResult> DeactivateByUser(Guid userId)
        {
            await _mediator.Send(new DeactivateProductsByUserCommand(userId));
            return Ok();
        }

        [HttpPut("activate-user/{userId}")]
        public async Task<ActionResult> ActivateByUser(Guid userId)
        {
            await _mediator.Send(new ActivateProductsByUserCommand(userId));
            return Ok();
        }
    }
}
