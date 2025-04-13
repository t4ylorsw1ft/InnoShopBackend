using InnoShop.UserService.Application.UseCases.Commands.EmailVerification.SendVerificationEmail;
using InnoShop.UserService.Application.UseCases.Commands.EmailVerification.VerifyEmail;
using InnoShop.UserService.Application.UseCases.Commands.Management.Activate;
using InnoShop.UserService.Application.UseCases.Commands.Management.Deactivate;
using InnoShop.UserService.Application.UseCases.Queries.IsUserActive;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.UserService.Presentation.Controllers
{
    [Route("api/users/")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPatch("deactivate")]
        public async Task<ActionResult> DeactivateUser(CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirst("userId")?.Value);
            await _mediator.Send(new DeactivateUserCommand(userId), cancellationToken);
            return Ok();
        }

        [Authorize]
        [HttpPatch("activate")]
        public async Task<ActionResult> ActivateUser(CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirst("userId")?.Value);
            await _mediator.Send(new ActivateUserCommand(userId), cancellationToken);
            return Ok();
        }

        [HttpGet("active-status/{id}")]
        public async Task<ActionResult<bool>> IsUserActive(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new IsUserActiveCommand(id), cancellationToken);
            return Ok(result);
        }
    }
}
