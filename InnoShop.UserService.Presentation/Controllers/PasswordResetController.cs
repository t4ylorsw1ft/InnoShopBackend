using InnoShop.UserService.Application.UseCases.Commands.EmailVerification.SendVerificationEmail;
using InnoShop.UserService.Application.UseCases.Commands.EmailVerification.VerifyEmail;
using InnoShop.UserService.Application.UseCases.Commands.PasswordReset.ResetPassword;
using InnoShop.UserService.Application.UseCases.Commands.PasswordReset.SendResetPasswordEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.UserService.Presentation.Controllers
{
    [Route("api/users/password-reset")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PasswordResetController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("request")]
        public async Task<ActionResult> SendResetPasswordEmail([FromBody] SendResetPasswordEmailCommand sendResetPasswordEmailCommand, CancellationToken cancellationToken)
        {
            await _mediator.Send(sendResetPasswordEmailCommand, cancellationToken);
            return Ok();
        }

        [HttpPost("reset")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordCommand resetPasswordCommand, CancellationToken cancellationToken)
        {
            await _mediator.Send(resetPasswordCommand, cancellationToken);
            return Ok();
        }

    }
}
