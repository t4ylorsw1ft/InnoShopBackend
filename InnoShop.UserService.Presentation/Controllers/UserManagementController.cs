using InnoShop.UserService.Application.UseCases.Commands.EmailVerification.SendVerificationEmail;
using InnoShop.UserService.Application.UseCases.Commands.EmailVerification.VerifyEmail;
using InnoShop.UserService.Application.UseCases.Commands.Management.Activate;
using InnoShop.UserService.Application.UseCases.Commands.Management.Deactivate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.UserService.Presentation.Controllers
{
    [Authorize]
    [Route("api/users/")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPatch("deactivate")]
        public async Task<ActionResult> DeactivateUser(CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirst("userId")?.Value);
            await _mediator.Send(new DeactivateUserCommand(userId), cancellationToken);
            return Ok();
        }

        [HttpPatch("activate")]
        public async Task<ActionResult> ActivateUser(CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirst("userId")?.Value);
            await _mediator.Send(new ActivateUserCommand(userId), cancellationToken);
            return Ok();
        }
    }
}
