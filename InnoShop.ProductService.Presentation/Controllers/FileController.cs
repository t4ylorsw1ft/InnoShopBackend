using InnoShop.ProductService.Application.UseCases.Files.Commands.DeleteImage;
using InnoShop.ProductService.Application.UseCases.Files.Commands.UploadImage;
using InnoShop.ProductService.Application.UseCases.Files.Queries.GetImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.ProductService.Presentation.Controllers
{
    public class FileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not uploaded.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream, cancellationToken);
            var fileData = stream.ToArray();

            var filePath = await _mediator.Send(new UploadImageCommand(fileData, file.FileName), cancellationToken);
            return Ok(new { filePath });
        }

        [HttpGet()]
        public async Task<IActionResult> GetFile([FromQuery] string filePath, CancellationToken cancellationToken)
        {
            var fileData = await _mediator.Send(new GetImageQuery(filePath), cancellationToken);
            return File(fileData, "application/octet-stream", Path.GetFileName(filePath));
        }

        [Authorize]
        [HttpDelete()]
        public async Task<IActionResult> DeleteFile([FromQuery] string filePath, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteImageCommand(filePath), cancellationToken);
            return Ok();
        }
    }
}
