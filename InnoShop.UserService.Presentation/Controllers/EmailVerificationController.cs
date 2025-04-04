using InnoShop.UserService.Application.UseCases.Commands.EmailVerification.SendVerificationEmail;
using InnoShop.UserService.Application.UseCases.Commands.EmailVerification.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.UserService.Presentation.Controllers
{
    [Authorize]
    [Route("api/users/email-verification")]
    [ApiController]
    public class EmailVerificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmailVerificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("request")]
        public async Task<ActionResult> SendVerificationEmail(CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirst("userId")?.Value);
            await _mediator.Send(new SendVerificationEmailCommand(userId), cancellationToken);
            return Ok();
        }

        [HttpPost("verify")]
        public async Task<ActionResult> VerifyEmail([FromBody] string code, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirst("userId")?.Value);
            await _mediator.Send(new VerifyEmailCommand(userId, code), cancellationToken);
            return Ok();
        }

    }
}
