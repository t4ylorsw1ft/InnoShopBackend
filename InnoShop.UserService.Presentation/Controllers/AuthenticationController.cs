using InnoShop.UserService.Application.UseCases.Commands.Authentication.Login;
using InnoShop.UserService.Application.UseCases.Commands.Authentication.Refresh;
using InnoShop.UserService.Application.UseCases.Commands.Authentication.Register;
using InnoShop.UserService.Application.UseCases.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.UserService.Presentation.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<JwtPairDto>> Register([FromBody] RegisterCommand registerCommand, CancellationToken cancellationToken)
        {
            var jwtPair = await _mediator.Send(registerCommand, cancellationToken);
            return Ok(jwtPair);
        }

        [HttpPost("login")]
        public async Task<ActionResult<JwtPairDto>> Login([FromBody] LoginCommand loginCommand, CancellationToken cancellationToken)
        {
            var jwtPair = await _mediator.Send(loginCommand, cancellationToken);
            return Ok(jwtPair);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<JwtPairDto>> Refresh([FromBody] RefreshCommand refreshCommand, CancellationToken cancellationToken)
        {
            var jwtPair = await _mediator.Send(refreshCommand, cancellationToken);
            return Ok(jwtPair);
        }
    }
}
